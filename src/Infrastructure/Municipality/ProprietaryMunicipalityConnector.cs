using Application.Municipality;
using AutoMapper;
using Domain.Enums;
using Domain.Models.Municipality;
using Domain.Models.Tax;
using Infrastructure.Tax.Data;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Municipality;

public class ProprietaryMunicipalityConnector : IMunicipalityConnector
{
    private readonly IMapper mapper;
    private readonly MunicipalityDbContext municipalityDbContext;
    private readonly Func<TaxRateDbContext> dbContext;

    public ProprietaryMunicipalityConnector(
        IMapper mapper,
        MunicipalityDbContext municipalityDbContext,
        Func<TaxRateDbContext> dbContext)
    {
        this.mapper = mapper;
        this.municipalityDbContext = municipalityDbContext;
        this.municipalityDbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        this.dbContext = dbContext;
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

        foreach (var entity in result)
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
}
