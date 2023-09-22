using Application.Tax.Proprietary.Abstractions;
using Application.Tax.Proprietary.Abstractions.Repositories;
using Domain.Models.Tax;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tax.Data;

public class StateTaxRateRepository : IStateTaxRateRepository
{
    private readonly Func<TaxRateDbContext> dbContextFunc;

    public StateTaxRateRepository(Func<TaxRateDbContext> dbContextFunc)
    {
        this.dbContextFunc = dbContextFunc;
    }

    public TaxRateEntity TaxRates(int calculationYear, int municipalityId)
    {
        using var dbContext = dbContextFunc();
        return dbContext.Rates.AsNoTracking()
            .FirstOrDefault(item => item.BfsId == municipalityId
                                    && item.Year == calculationYear);
    }
}
