using System;
using System.Collections.Generic;
using System.Linq;
using Tax.Data.Abstractions;
using Tax.Data.Abstractions.Models;


namespace Tax.Data
{
    public class TaxTariffData : ITaxTariffData
    {
        private readonly Func<TaxTariffDbContext> _dbContext;

        public TaxTariffData(Func<TaxTariffDbContext> dbContext)
        {
            _dbContext = dbContext;
        }

        public IReadOnlyCollection<TaxTariffModel> Get(TaxFilterModel filter)
        {
            using var ctxt = _dbContext();
            return ctxt.Tariffs
                .Where(item => item.Canton == filter.Canton && item.Year == filter.Year)
                .ToList();
        }
    }
}