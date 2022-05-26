using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using AutoMapper;
using LanguageExt;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.CommonUtils;
using PensionCoach.Tools.PostOpenApi;
using PensionCoach.Tools.PostOpenApi.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using Tax.Data;
using Tax.Data.Abstractions.Models;

namespace TaxCalculator
{
    public class MunicipalityConnector : IMunicipalityConnector
    {
        private readonly IMapper mapper;
        private readonly MunicipalityDbContext municipalityDbContext;
        private readonly Func<TaxRateDbContext> dbContext;
        private readonly IPostOpenApiClient postOpenApiClient;
        private readonly ILogger<MunicipalityConnector> logger;

        public MunicipalityConnector(
            IMapper mapper,
            MunicipalityDbContext municipalityDbContext,
            Func<TaxRateDbContext> dbContext,
            IPostOpenApiClient postOpenApiClient,
            ILogger<MunicipalityConnector> logger)
        {
            this.mapper = mapper;
            this.municipalityDbContext = municipalityDbContext;
            this.dbContext = dbContext;
            this.postOpenApiClient = postOpenApiClient;
            this.logger = logger;
        }

        private bool HasFetchFinished { get; set; }

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

        /// <inheritdoc />
        public async Task<IEnumerable<ZipModel>> GetAllZipCodesAsync()
        {
            const int numberOfReaders = 5;
            const int limitPerFetch = 100;

            using (new MeasureTime(t => logger.LogDebug($"Execution time to fetch all ZIP: {t}ms")))
            {
                Channel<(int, int)> fetchZipChannel = Channel.CreateBounded<(int, int)>(new BoundedChannelOptions(5));
                Channel<ZipModel> resultZipChannel = Channel.CreateUnbounded<ZipModel>();

                var consumers =
                    Enumerable
                        .Range(1, numberOfReaders)
                        .Select(counter => ConsumeDataAsync(fetchZipChannel.Reader, resultZipChannel.Writer, counter))
                        .ToArray();

                await Task.Run(async () =>
                {
                    int count = 0;
                    while (!HasFetchFinished)
                    {
                        await fetchZipChannel.Writer.WriteAsync((limitPerFetch, count * limitPerFetch));
                        count++;
                    }

                    fetchZipChannel.Writer.Complete();
                });

                await Task.WhenAll(consumers);

                resultZipChannel.Writer.Complete();

                List<ZipModel> zipModels = new();

                await foreach (var record in resultZipChannel.Reader.ReadAllAsync(CancellationToken.None))
                {
                    zipModels.Add(record);
                }

                return zipModels;
            }
        }

        private async Task ConsumeDataAsync(ChannelReader<(int, int)> channelReader, ChannelWriter<ZipModel> channelWriter, int readerId)
        {
            while (await channelReader.WaitToReadAsync())
            {
                if (channelReader.TryRead(out (int limit, int offset) fetch))
                {
                    logger.LogDebug($"Fetch zip data by reader {readerId}: limit {fetch.limit}, offset {fetch.offset}");
                    List<ZipModel> records = new List<ZipModel>();
                    foreach (ZipModel model in await postOpenApiClient.GetZipCodesAsync(fetch.limit, fetch.offset)
                                 switch
                                 {
                                     null => Array.Empty<ZipModel>(),
                                     { TotalCount: 0 } => Array.Empty<ZipModel>(),
                                     { Records: { } } r => r.Records.Select(x => new ZipModel
                                     {
                                         BfsCode = x.Record.Fields.BfsCode,
                                         MunicipalityName = x.Record.Fields.MunicipalityName,
                                         Canton = x.Record.Fields.Canton,
                                         ZipCode = x.Record.Fields.ZipCode,
                                         DateOfValidity = DateTime.Parse(x.Record.Fields.DateOfValidity, CultureInfo.InvariantCulture),
                                     })
                                 })
                    {
                        records.Add(model);
                    }

                    records.Iter(item => channelWriter.WriteAsync(item));

                    if (records.Length() == 0)
                    {
                        HasFetchFinished = true;
                    }
                }
            }

            logger.LogDebug($"Reader {readerId} completed");
        }
    }
}
