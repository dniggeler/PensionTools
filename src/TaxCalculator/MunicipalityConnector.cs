using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using AutoMapper;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.CommonTypes.Tax;
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

        public MunicipalityConnector(
            IMapper mapper,
            MunicipalityDbContext municipalityDbContext,
            Func<TaxRateDbContext> dbContext,
            IPostOpenApiClient postOpenApiClient)
        {
            this.mapper = mapper;
            this.municipalityDbContext = municipalityDbContext;
            this.dbContext = dbContext;
            this.postOpenApiClient = postOpenApiClient;
        }

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
            const int limitPerFetch = 100;
            int offsetFactor = 0;

            Channel<(int, int)> fetchZipChannel = Channel.CreateBounded<(int, int)>(new BoundedChannelOptions(5));

            _ = Task.Run(async () =>
            {
                for (int ii = 0; ii < 10; ii++)
                {
                    await fetchZipChannel.Writer.WriteAsync((limitPerFetch, offsetFactor));
                }
            });

            int count = 0;

            List<ZipModel> zipModels = new();
            while (count < 10)
            {
                (int limit, int offset) = await fetchZipChannel.Reader.ReadAsync();

                IEnumerable<ZipModel> zipBatch =
                    await postOpenApiClient.GetZipCodesAsync(limit, offset) switch
                    {
                        null => Array.Empty<ZipModel>(),
                        { Records: { } } r => r.Records.Select(x => new ZipModel
                        {
                            BfsCode = x.Record.Fields.BfsCode,
                            MunicipalityName = x.Record.Fields.MunicipalityName,
                            Canton = x.Record.Fields.Canton,
                            ZipCode = x.Record.Fields.ZipCode,
                            DateOfValidity = DateTime.Parse(x.Record.Fields.DateOfValidity,
                                CultureInfo.InvariantCulture),
                        })
                    };

                zipModels.AddRange(zipBatch);

                count++;
            }

            return zipModels;
        }
    }
}
