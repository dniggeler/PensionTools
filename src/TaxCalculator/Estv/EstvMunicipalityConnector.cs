using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using Tax.Data;
using Tax.Data.Abstractions.Models;

namespace PensionCoach.Tools.TaxCalculator.Estv;

public class EstvMunicipalityConnector : IMunicipalityConnector
{
    private readonly int[] supportedTaxYears = { 2019, 2020, 2021, 2022 };

    private readonly IMapper mapper;
    private readonly MunicipalityDbContext municipalityDbContext;

    public EstvMunicipalityConnector(
        IMapper mapper,
        MunicipalityDbContext municipalityDbContext)
    {
        this.mapper = mapper;
        this.municipalityDbContext = municipalityDbContext;
        this.municipalityDbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
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

    public Task<Either<string, MunicipalityModel>> GetAsync(int bfsNumber, int year)
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
        const int maxEstvSupportedYear = 2022;

        IReadOnlyCollection<TaxSupportedMunicipalityModel> list = municipalityDbContext.MunicipalityEntities
            .Where(item => item.TaxLocationId != null)
            .OrderBy(item => item.Canton)
            .ThenBy(item => item.Name)
            .Select(item => new TaxSupportedMunicipalityModel
            {
                BfsMunicipalityNumber = item.BfsNumber,
                Name = item.Name,
                Canton = Enum.Parse<Canton>(item.Canton),
                MaxSupportedYear = maxEstvSupportedYear,
                EstvTaxLocationId = item.TaxLocationId
            })
            .ToList();

        return Task.FromResult(list);
    }

    public Task<int[]> GetSupportedTaxYearsAsync()
    {
        return supportedTaxYears.AsTask();
    }
}
