using Application.Municipality;
using Domain.Enums;
using Domain.Models.Municipality;
using Infrastructure.Tax.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Municipality;

public class MunicipalityRepository : IMunicipalityRepository
{
    private readonly MunicipalityDbContext municipalityDbContext;

    public MunicipalityRepository(MunicipalityDbContext municipalityDbContext)
    {
        this.municipalityDbContext = municipalityDbContext;
        this.municipalityDbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public IEnumerable<MunicipalityEntity> GetAll()
    {
        return municipalityDbContext.MunicipalityEntities.ToList();
    }

    public MunicipalityEntity Get(int bfsNumber, int year)
    {
        return municipalityDbContext.MunicipalityEntities
            .FirstOrDefault(item => item.BfsNumber == bfsNumber && string.IsNullOrEmpty(item.DateOfMutation));
    }

    public IEnumerable<MunicipalityEntity> GetAllSupportTaxCalculation()
    {
        return municipalityDbContext.MunicipalityEntities
            .Where(item => item.TaxLocationId != null)
            .OrderBy(item => item.Canton)
            .ThenBy(item => item.Name);
    }

    public IEnumerable<MunicipalityEntity> Search(MunicipalitySearchFilter searchFilter)
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

        return result;
    }
}
