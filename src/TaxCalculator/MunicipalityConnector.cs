using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

namespace TaxCalculator;

public class MunicipalityConnector : IMunicipalityConnector
{
    private readonly IMapper mapper;
    private readonly MunicipalityDbContext municipalityDbContext;
    private readonly Func<TaxRateDbContext> dbContext;

    public MunicipalityConnector(
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
}
