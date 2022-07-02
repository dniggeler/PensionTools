using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AutoMapper;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.CommonUtils;
using PensionCoach.Tools.EstvTaxCalculators;
using PensionCoach.Tools.EstvTaxCalculators.Models;
using PensionCoach.Tools.PostOpenApi;
using PensionCoach.Tools.PostOpenApi.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using Tax.Data;
using Tax.Data.Abstractions.Models;
using TaxCalculator.Internals;

namespace TaxCalculator
{
    internal enum SearchResultType
    {
        None,
        Match,
        NotUnique
    }

    public class MunicipalityConnector : IMunicipalityConnector
    {
        private readonly IMapper mapper;
        private readonly MunicipalityDbContext municipalityDbContext;
        private readonly Func<TaxRateDbContext> dbContext;
        private readonly IEstvTaxCalculatorClient estvTaxCalculatorClient;
        private readonly IPostOpenApiClient postOpenApiClient;
        private readonly ILogger<MunicipalityConnector> logger;

        public MunicipalityConnector(
            IMapper mapper,
            MunicipalityDbContext municipalityDbContext,
            Func<TaxRateDbContext> dbContext,
            IEstvTaxCalculatorClient estvTaxCalculatorClient,
            IPostOpenApiClient postOpenApiClient,
            ILogger<MunicipalityConnector> logger)
        {
            this.mapper = mapper;
            this.municipalityDbContext = municipalityDbContext;
            this.municipalityDbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            this.dbContext = dbContext;
            this.estvTaxCalculatorClient = estvTaxCalculatorClient;
            this.postOpenApiClient = postOpenApiClient;
            this.logger = logger;
        }

        private int TotalCount { get; set; }

        public Task<IEnumerable<MunicipalityModel>> GetAllAsync()
        {
            return Task.FromResult(
                mapper.Map<IEnumerable<MunicipalityModel>>(
                    municipalityDbContext.MunicipalityEntities.ToList()));
        }

        /// <summary>
        /// Searches the specified search filter.
        /// </summary>
        /// <param name="searchFilter">The search filter.</param>
        /// <returns>List of municipalities.</returns>
        public IEnumerable<MunicipalityModel> Search(MunicipalitySearchFilter searchFilter)
        {
            IQueryable<MunicipalityEntity> result = municipalityDbContext.MunicipalityEntities;

            if (searchFilter.Canton != Canton.Undefined)
            {
                result = result.Where(item => item.Canton == searchFilter.Canton.ToString());
            }

            if (!string.IsNullOrEmpty(searchFilter.Name))
            {
                result =
                    result.Where(item => item.Name.Contains(searchFilter.Name));
            }

            foreach (MunicipalityEntity entity in result)
            {
                var model = mapper.Map<MunicipalityModel>(entity);

                if (searchFilter.YearOfValidity.HasValue)
                {
                    if (!model.DateOfMutation.HasValue)
                    {
                        yield return model;
                    }
                    else if (model.DateOfMutation.Value.Year > searchFilter.YearOfValidity)
                    {
                        yield return model;
                    }
                }
                else
                {
                    yield return model;
                }
            }
        }

        public Task<Either<string, MunicipalityModel>> GetAsync(
            int bfsNumber, int year)
        {
            Option<MunicipalityEntity> entity =
                municipalityDbContext.MunicipalityEntities
                    .FirstOrDefault(item => item.BfsNumber == bfsNumber
                                            && string.IsNullOrEmpty(item.DateOfMutation));

            return entity
                .Match<Either<string, MunicipalityModel>>(
                    Some: item => mapper.Map<MunicipalityModel>(item),
                    None: () => $"Municipality not found by BFS number {bfsNumber}")
                .AsTask();
        }

        /// <inheritdoc />
        public Task<IReadOnlyCollection<TaxSupportedMunicipalityModel>> GetAllSupportTaxCalculationAsync()
        {
            using var ctx = dbContext();
            IReadOnlyCollection<TaxSupportedMunicipalityModel> municipalities =
                ctx.Rates
                    .AsEnumerable()
                    .GroupBy(keySelector => new
                    {
                        Id = keySelector.BfsId,
                        Name = keySelector.MunicipalityName,
                        keySelector.Canton,
                    })
                    .Select(item => new TaxSupportedMunicipalityModel
                    {
                        BfsMunicipalityNumber = item.Key.Id,
                        Name = item.Key.Name,
                        Canton = Enum.Parse<Canton>(item.Key.Canton),
                        MaxSupportedYear = item.Max(entity => entity.Year),
                    })
                    .OrderBy(item => item.Name)
                    .ToList();

            return Task.FromResult(municipalities);
        }

        /// <param name="limit"></param>
        /// <inheritdoc />
        public async IAsyncEnumerable<ZipModel> GetAllZipCodesAsync(int limit)
        {
            const int numberOfReaders = 5;
            const int limitPerFetch = 100;

            using (new MeasureTime(t => logger.LogDebug($"Execution time to fetch all ZIP: {t}ms")))
            {
                logger.LogDebug($"First fetch zip data ({limitPerFetch} items)");

                OpenApiZipInfo openApiData = await postOpenApiClient.GetZipCodesAsync(limitPerFetch, 0);

                if (openApiData is not { TotalCount: > 0 })
                {
                    yield break;
                }

                TotalCount = openApiData.TotalCount;

                Channel<(int, int)> fetchZipChannel = Channel.CreateBounded<(int, int)>(new BoundedChannelOptions(5));
                Channel<ZipModel> resultZipChannel = Channel.CreateUnbounded<ZipModel>();

                Task[] consumers = Enumerable
                    .Range(1, numberOfReaders)
                    .Select(counter => ConsumeDataAsync(fetchZipChannel.Reader, resultZipChannel.Writer, counter))
                    .ToArray();

                await WriteOpenApiDataToChannel(resultZipChannel, openApiData);

                await Task.Run(async () =>
                {
                    int count = 1;
                    while (count * limitPerFetch < TotalCount)
                    {
                        await fetchZipChannel.Writer.WriteAsync((limitPerFetch, count * limitPerFetch));
                        count++;
                    }

                    fetchZipChannel.Writer.Complete();
                });

                await Task.WhenAll(consumers);

                resultZipChannel.Writer.Complete();

                await foreach (ZipModel record in resultZipChannel.Reader.ReadAllAsync(CancellationToken.None))
                {
                    yield return record;
                }
            }
        }

        public async Task<int> PopulateWithZipCodeAsync()
        {
            int count = 0;
            foreach (var municipalityEntity in municipalityDbContext.MunicipalityEntities
                         .Where(item => item.SuccessorId == 0 && string.IsNullOrEmpty(item.ZipCode)))
            {
                ZipEntity[] zipEntities = municipalityDbContext.TaxMunicipalityEntities
                    .Where(item => item.BfsNumber == municipalityEntity.BfsNumber && item.Canton == municipalityEntity.Canton)
                    .ToArray();

                if (zipEntities.Length == 1) // unique, easy case
                {
                    if (municipalityDbContext.MunicipalityEntities.Local
                        .Any(item =>
                            item.BfsNumber == municipalityEntity.BfsNumber &&
                            item.MutationId == municipalityEntity.MutationId))
                    {
                        continue;
                    }

                    municipalityEntity.ZipCode = zipEntities[0].ZipCode;
                    municipalityDbContext.Update(municipalityEntity);
                    count++;
                }
                else if (zipEntities.Length > 1)
                {
                    var zipsByNameEntities = zipEntities
                        .Where(item => item.Name.Contains(municipalityEntity.CleanName))
                        .ToArray();

                    if (zipsByNameEntities.Length == 1)
                    {
                        if (municipalityDbContext.MunicipalityEntities.Local
                            .Any(item =>
                                item.BfsNumber == municipalityEntity.BfsNumber &&
                                item.MutationId == municipalityEntity.MutationId))
                        {
                            continue;
                        }

                        municipalityEntity.ZipCode = zipsByNameEntities[0].ZipCode;
                        municipalityDbContext.Update(municipalityEntity);
                        count++;
                    }
                    else if (zipsByNameEntities.Length > 1)
                    {
                        // take smallest zip add on
                        var zipsByAddOn = zipsByNameEntities
                            .Where(item => item.ZipCodeAddOn == "0")
                            .ToArray();

                        if (zipsByAddOn.Length == 1)
                        {
                            if (municipalityDbContext.MunicipalityEntities.Local
                                .Any(item =>
                                    item.BfsNumber == municipalityEntity.BfsNumber &&
                                    item.MutationId == municipalityEntity.MutationId))
                            {
                                continue;
                            }

                            municipalityEntity.ZipCode = zipsByAddOn[0].ZipCode;
                            municipalityDbContext.Update(municipalityEntity);
                            count++;
                        }
                        else if (zipsByAddOn.Length > 1)
                        {
                            // take smallest zip add on
                            var zipFinalEntity = zipsByAddOn
                                .OrderBy(item => item.Name.Length)
                                .First();

                            if (municipalityDbContext.MunicipalityEntities.Local
                                .Any(item =>
                                    item.BfsNumber == municipalityEntity.BfsNumber &&
                                    item.MutationId == municipalityEntity.MutationId))
                            {
                                continue;
                            }

                            municipalityEntity.ZipCode = zipFinalEntity.ZipCode;
                            municipalityDbContext.Update(municipalityEntity);
                            count++;
                        }
                    }
                }
            }

            await municipalityDbContext.SaveChangesAsync();

            return count;
        }

        public async Task<int> PopulateWithTaxLocationAsync(bool doClear)
        {
            const int numberOfReaders = 5;

            if (doClear)
            {
                foreach (var municipalityEntity in municipalityDbContext.MunicipalityEntities.AsNoTracking())
                {
                    municipalityEntity.TaxLocationId = null;
                    municipalityEntity.Remark = null;
                    municipalityDbContext.Update(municipalityEntity);
                }

                await municipalityDbContext.SaveChangesAsync(CancellationToken.None);

                municipalityDbContext.ChangeTracker.Clear();
            }

            Channel<MunicipalityEntity> estvFetchChannel = Channel.CreateBounded<MunicipalityEntity>(new BoundedChannelOptions(5));
            Channel<TaxLocationSearchHolder> notUniqueChannel = Channel.CreateBounded<TaxLocationSearchHolder>(new BoundedChannelOptions(5));
            Channel<TaxLocationSearchHolder> notFoundChannel = Channel.CreateBounded<TaxLocationSearchHolder>(new BoundedChannelOptions(5));

            Channel<TaxLocationSearchHolder> dbStoreChannel = Channel.CreateUnbounded<TaxLocationSearchHolder>();

            Task[] fetchConsumers = Enumerable
                .Range(1, numberOfReaders)
                .Select(_ => FetchConsumerAsync())
                .ToArray();

            Task[] notFoundConsumers = Enumerable
                .Range(1, numberOfReaders)
                .Select(_ => NotFoundConsumerAsync())
                .ToArray();

            Task[] notUniqueConsumers = Enumerable
                .Range(1, numberOfReaders)
                .Select(_ => NotUniqueConsumerAsync())
                .ToArray();

            int count = 0;

            await Task.Run(
                async () =>
                {
                    foreach (MunicipalityEntity model in municipalityDbContext.MunicipalityEntities
                                 .Where(item => item.SuccessorId == 0 &&
                                                item.MutationType == 11 &&
                                                item.TaxLocationId == null))
                    {
                        await estvFetchChannel.Writer.WriteAsync(model);
                    }

                    estvFetchChannel.Writer.Complete();
                });

            await Task.WhenAll(fetchConsumers);

            notFoundChannel.Writer.Complete();
            notUniqueChannel.Writer.Complete();

            await Task.WhenAll(notFoundConsumers.Union(notUniqueConsumers));

            dbStoreChannel.Writer.Complete();

            return await StoreResults(dbStoreChannel);

            async Task FetchConsumerAsync()
            {
                while (await estvFetchChannel.Reader.WaitToReadAsync())
                {
                    if (estvFetchChannel.Reader.TryRead(out MunicipalityEntity entity))
                    {
                        TaxLocation[] locations = await estvTaxCalculatorClient.GetTaxLocationsAsync(entity.ZipCode, entity.CleanName);
                        TaxLocationSearchHolder searchHolder = CreateSearchHolder(locations, entity);

                        switch (searchHolder.SearchResultType)
                        {
                            case SearchResultType.None: await notFoundChannel.Writer.WriteAsync(searchHolder);
                                break;
                            case SearchResultType.NotUnique:
                                await notUniqueChannel.Writer.WriteAsync(searchHolder);
                                break;
                            default:
                                await dbStoreChannel.Writer.WriteAsync(searchHolder);
                                break;
                        }
                    }
                }
            }

            async Task NotFoundConsumerAsync()
            {
                while (await notFoundChannel.Reader.WaitToReadAsync())
                {
                    if (!notFoundChannel.Reader.TryRead(out TaxLocationSearchHolder searchHolder))
                    {
                        continue;
                    }

                    // just try part of name before a hypen (ie. Illnau-Effretikon)
                    string[] nameParts = searchHolder.MunicipalityEntity.CleanName.Split(new[] { '-', ' ', '/' });

                    if (nameParts.Length == 2)
                    {
                        TaxLocation[] locationsByPart =
                            await estvTaxCalculatorClient.GetTaxLocationsAsync(searchHolder.MunicipalityEntity.ZipCode, nameParts[0]);

                        TaxLocationSearchHolder newSearchHolder = CreateSearchHolder(locationsByPart, searchHolder.MunicipalityEntity);

                        switch (newSearchHolder.SearchResultType)
                        {
                            case SearchResultType.Match:
                            case SearchResultType.None:
                                await dbStoreChannel.Writer.WriteAsync(newSearchHolder);
                                break;
                            case SearchResultType.NotUnique:

                                TaxLocation[] nameWithCantons = MatchCanton(locationsByPart, searchHolder.MunicipalityEntity);
                                TaxLocationSearchHolder byCantonSearchHolder = CreateSearchHolder(nameWithCantons, searchHolder.MunicipalityEntity);

                                await dbStoreChannel.Writer.WriteAsync(byCantonSearchHolder);
                                break;
                        }
                    }
                    else if (searchHolder.MunicipalityEntity.ZipCode is not null)
                    {
                        TaxLocation[] zipOnly =
                            await estvTaxCalculatorClient.GetTaxLocationsAsync(searchHolder.MunicipalityEntity.ZipCode, string.Empty);

                        TaxLocationSearchHolder zipOnlySearchHolder = CreateSearchHolder(zipOnly, searchHolder.MunicipalityEntity);

                        switch (zipOnlySearchHolder.SearchResultType)
                        {
                            case SearchResultType.Match:
                            case SearchResultType.None:
                                await dbStoreChannel.Writer.WriteAsync(zipOnlySearchHolder);
                                break;
                            case SearchResultType.NotUnique:

                                TaxLocation[] nameWithCantons = MatchCanton(zipOnly, searchHolder.MunicipalityEntity);
                                TaxLocationSearchHolder byCantonSearchHolder = CreateSearchHolder(nameWithCantons, searchHolder.MunicipalityEntity);

                                switch (byCantonSearchHolder.SearchResultType)
                                {
                                    case SearchResultType.Match:
                                    case SearchResultType.None:
                                        await dbStoreChannel.Writer.WriteAsync(byCantonSearchHolder);
                                        break;
                                    case SearchResultType.NotUnique:
                                        await notUniqueChannel.Writer.WriteAsync(byCantonSearchHolder);
                                        break;
                                }

                                break;
                        }
                    }
                    else if (searchHolder.MunicipalityEntity.ZipCode is null)
                    {
                        TaxLocation[] nameOnly =
                            await estvTaxCalculatorClient.GetTaxLocationsAsync(string.Empty, searchHolder.MunicipalityEntity.CleanName);

                        TaxLocationSearchHolder nameOnlySearchHolder = CreateSearchHolder(nameOnly, searchHolder.MunicipalityEntity);

                        switch (nameOnlySearchHolder.SearchResultType)
                        {
                            case SearchResultType.Match:
                            case SearchResultType.None:
                                await dbStoreChannel.Writer.WriteAsync(nameOnlySearchHolder);
                                break;
                            case SearchResultType.NotUnique:

                                TaxLocation[] nameWithCantons = MatchCanton(nameOnly, searchHolder.MunicipalityEntity);
                                TaxLocationSearchHolder byCantonSearchHolder = CreateSearchHolder(nameWithCantons, searchHolder.MunicipalityEntity);

                                await dbStoreChannel.Writer.WriteAsync(byCantonSearchHolder);

                                break;
                        }
                    }
                    else
                    {
                        await dbStoreChannel.Writer.WriteAsync(searchHolder with
                        {
                            SearchResultType = SearchResultType.None,
                            SearchLevel = searchHolder.SearchLevel + 1,
                        });
                    }
                }
            }

            async Task NotUniqueConsumerAsync()
            {
                while (await notUniqueChannel.Reader.WaitToReadAsync())
                {
                    if (notUniqueChannel.Reader.TryRead(out TaxLocationSearchHolder searchHolder))
                    {
                        if (searchHolder.TaxLocations == null)
                        {
                            continue;
                        }

                        TaxLocation[] matchLocations = MatchExactName(searchHolder.TaxLocations, searchHolder.MunicipalityEntity);
                        TaxLocationSearchHolder cityOnlySearchHolder = CreateSearchHolder(matchLocations, searchHolder.MunicipalityEntity);

                        switch (cityOnlySearchHolder.SearchResultType)
                        {
                            case SearchResultType.Match:

                                await dbStoreChannel.Writer.WriteAsync(cityOnlySearchHolder);
                                break;
                            case SearchResultType.None:

                                TaxLocation[] approxNameLocations = MatchContainsNameAndCanton(searchHolder.TaxLocations, searchHolder.MunicipalityEntity);
                                TaxLocationSearchHolder approxNameSearchHolder = CreateSearchHolder(approxNameLocations, cityOnlySearchHolder.MunicipalityEntity);

                                switch (approxNameSearchHolder.SearchResultType)
                                {
                                    case SearchResultType.NotUnique:

                                        TaxLocation[] taxIdLocations = MatchByZipAsTaxId(searchHolder.TaxLocations, searchHolder.MunicipalityEntity);
                                        TaxLocationSearchHolder taxIdMatchSearchHolder = CreateSearchHolder(taxIdLocations, approxNameSearchHolder.MunicipalityEntity);

                                        await dbStoreChannel.Writer.WriteAsync(taxIdMatchSearchHolder);
                                        break;

                                    default:
                                        await dbStoreChannel.Writer.WriteAsync(approxNameSearchHolder);
                                        break;
                                }

                                break;
                            case SearchResultType.NotUnique:

                                TaxLocation[] nameWithCantons = MatchContainsNameAndCanton(matchLocations, searchHolder.MunicipalityEntity);
                                TaxLocationSearchHolder byCantonSearchHolder = CreateSearchHolder(nameWithCantons, searchHolder.MunicipalityEntity);

                                await dbStoreChannel.Writer.WriteAsync(byCantonSearchHolder);

                                break;
                        }
                    }
                }
            }
        }

        private async Task<int> StoreResults(Channel<TaxLocationSearchHolder> dbStoreChannel)
        {
            int count = 0;
            await foreach (TaxLocationSearchHolder searchHolder in dbStoreChannel.Reader.ReadAllAsync(CancellationToken.None))
            {
                // not found
                if (searchHolder.SearchResultType is SearchResultType.None or SearchResultType.NotUnique)
                {
                    searchHolder.MunicipalityEntity.Remark = searchHolder.SearchResultType.ToString();
                    municipalityDbContext.Update(searchHolder.MunicipalityEntity);
                    continue;
                }

                searchHolder.MunicipalityEntity.TaxLocationId = searchHolder.TaxLocations[0].Id;
                if (searchHolder.MunicipalityEntity.ZipCode is null && searchHolder.TaxLocations[0].ZipCode is not null)
                {
                    searchHolder.MunicipalityEntity.ZipCode = searchHolder.TaxLocations[0].ZipCode;
                }

                municipalityDbContext.Update(searchHolder.MunicipalityEntity);

                count++;
            }

            await municipalityDbContext.SaveChangesAsync(CancellationToken.None);

            return count;
        }

        public async Task<int> StagePlzTableAsync()
        {
            municipalityDbContext.TruncateTaxMunicipalityTable();

            int count = 0;

            await foreach (var model in GetAllZipCodesAsync(int.MaxValue))
            {
                municipalityDbContext.Add(new ZipEntity
                {
                    BfsNumber = model.BfsCode,
                    Canton = model.Canton,
                    ZipCode = model.ZipCode,
                    ZipCodeAddOn = model.ZipCodeAddOn,
                    Name = model.MunicipalityName,
                    LanguageCode = model.LanguageCode,
                    DateOfValidity = model.DateOfValidity,
                });

                count++;
            }

            await municipalityDbContext.SaveChangesAsync(CancellationToken.None);

            return count;
        }

        public async Task<int> CleanMunicipalityName()
        {
            int count = 0;
            foreach (var municipalityEntity in municipalityDbContext.MunicipalityEntities)
            {
                Prelude.Optional((municipalityEntity.Name, municipalityEntity.Canton))
                    .Map(t => RemoveCantonDescription(t.Name, t.Canton))
                    .Map(Abbreviate)
                    .Iter(cleanName =>
                    {
                        municipalityEntity.CleanName = cleanName;
                        municipalityDbContext.Update(municipalityEntity);

                        count++;
                    });
            }

            await municipalityDbContext.SaveChangesAsync(CancellationToken.None);

            return count;
        }

        private static async Task WriteOpenApiDataToChannel(ChannelWriter<ZipModel> channelWriter, OpenApiZipInfo openApiData)
        {
            foreach (ZipModel model in openApiData.Records.Select(x => new ZipModel
                     {
                         BfsCode = x.Record.Fields.BfsCode,
                         MunicipalityName = x.Record.Fields.MunicipalityName,
                         Canton = x.Record.Fields.Canton,
                         ZipCode = x.Record.Fields.ZipCode,
                         ZipCodeAddOn = x.Record.Fields.ZipCodeAddOn,
                         LanguageCode = x.Record.Fields.LanguageCode,
                         DateOfValidity = DateTime.Parse(x.Record.Fields.DateOfValidity, CultureInfo.InvariantCulture),
                     }))
            {
                await channelWriter.WriteAsync(model);
            }
        }

        private async Task ConsumeDataAsync(ChannelReader<(int, int)> channelReader, ChannelWriter<ZipModel> channelWriter, int readerId)
        {
            while (await channelReader.WaitToReadAsync())
            {
                if (channelReader.TryRead(out (int limit, int offset) fetch))
                {
                    logger.LogDebug($"Fetch zip data by reader {readerId}: limit {fetch.limit}, offset {fetch.offset}");

                    OpenApiZipInfo openApiData = await postOpenApiClient.GetZipCodesAsync(fetch.limit, fetch.offset);

                    if (openApiData is null || openApiData.Records.Length() == 0)
                    {
                        break;
                    }

                    await WriteOpenApiDataToChannel(channelWriter, openApiData);
                }
            }

            logger.LogDebug($"Reader {readerId} completed");
        }

        private string RemoveCantonDescription(string name, string canton)
        {
            return name
                .Replace($" ({canton})", string.Empty)
                .Replace($" {canton}", string.Empty)
                .Trim();
        }

        private string Abbreviate(string name)
        {
            return name
                .Replace(" bei ", " b. ")
                .Replace("Saint-", "St-")
                .Replace("Sainte-", "Ste-")
                .Replace("ë", "e")
                .Trim();
        }

        private TaxLocationSearchHolder CreateSearchHolder(TaxLocation[] locations, MunicipalityEntity entity)
        {
            if (locations.Length > 1)
            {
                return new TaxLocationSearchHolder
                {
                    MunicipalityEntity = entity,
                    TaxLocations = locations,
                    SearchResultType = SearchResultType.NotUnique,
                    SearchLevel = 1,
                };
            }

            if (locations.Length == 0)
            {
                return new TaxLocationSearchHolder
                {
                    MunicipalityEntity = entity, SearchResultType = SearchResultType.None, SearchLevel = 1,
                };
            }

            return new TaxLocationSearchHolder
            {
                MunicipalityEntity = entity,
                SearchResultType = SearchResultType.Match,
                TaxLocations = locations,
            };
        }

        private TaxLocation[] MatchExactZipAndName(TaxLocation[] sourceLocations, MunicipalityEntity entity)
        {
            return sourceLocations
                .Where(item => item.ZipCode == entity.ZipCode && item.City == entity.CleanName)
                .ToArray();
        }

        private TaxLocation[] MatchExactName(TaxLocation[] sourceLocations, MunicipalityEntity entity)
        {
            return sourceLocations
                .Where(item => item.City == entity.CleanName)
                .ToArray();
        }

        private TaxLocation[] MatchCanton(TaxLocation[] sourceLocations, MunicipalityEntity entity)
        {
            return sourceLocations
                .Where(item => item.Canton == entity.Canton)
                .ToArray();
        }

        private TaxLocation[] MatchContainsNameAndCanton(TaxLocation[] sourceLocations, MunicipalityEntity entity)
        {
            return sourceLocations
                .Where(item => item.City.Contains(entity.CleanName) && item.Canton == entity.Canton)
                .ToArray();
        }

        private TaxLocation[] MatchByZipAsTaxId(TaxLocation[] sourceLocations, MunicipalityEntity entity)
        {
            int taxIdFromZipCode = Convert.ToInt32(entity.ZipCode) * 100000;

            return sourceLocations
                .Where(item => item.Id == taxIdFromZipCode)
                .ToArray();
        }
    }
}
