﻿using Application.Tax.Proprietary.Repositories;
using Domain.Models.Tax;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tax.Data
{
    public class StateTaxRateRepository : IStateTaxRateRepository
    {
        private readonly Func<TaxRateDbContext> dbContextFunc;

        public StateTaxRateRepository(Func<TaxRateDbContext> dbContextFunc)
        {
            this.dbContextFunc = dbContextFunc;
        }

        public IEnumerable<TaxRateEntity> TaxRates()
        {
            using var dbContext = dbContextFunc();
            return dbContext.Rates.AsNoTracking().ToList();
        }

        public TaxRateEntity TaxRates(int calculationYear, int municipalityId)
        {
            using var dbContext = dbContextFunc();
            return dbContext.Rates.AsNoTracking()
                .FirstOrDefault(item => item.BfsId == municipalityId && item.Year == calculationYear);
        }
    }
}
