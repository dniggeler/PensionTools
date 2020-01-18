using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tax.Data.Abstractions;
using Tax.Data.Abstractions.Models;


namespace Tax.Data
{
    public class TaxTariffData : ITaxTariffData
    {
        private readonly Func<TaxTariffDbContext> dbContext;

        public TaxTariffData(Func<TaxTariffDbContext> dbContext)
        {
            this.dbContext = dbContext;
        }

        public IReadOnlyCollection<TaxTariffModel> Get(TaxFilterModel filter)
        {
            using var ctxt = this.dbContext();
            return ctxt.Tariffs.AsNoTracking()
                .Where(item => item.Canton == filter.Canton
                               && item.Year == filter.Year)
                .ToList();
        }
    }
}