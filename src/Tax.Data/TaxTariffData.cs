using System;
using System.Collections.Generic;
using System.Linq;
using Tax.Data.Abstractions;
using Tax.Data.Abstractions.Models;


namespace Tax.Data
{
    public class TaxTariffData : ITaxTariffData
    {
        private readonly Func<TaxTariffDbContext> _dbContextFunc;

        public TaxTariffData(Func<TaxTariffDbContext> dbContextFunc)
        {
            _dbContextFunc = dbContextFunc;
        }
        public IReadOnlyCollection<TaxTariffModel> Get(TaxFilterModel filter)
        {
            using (var dbContext = _dbContextFunc())
            {
                return dbContext.Tariffs
                    .Where(item => item.Canton == filter.Canton && item.Year == filter.Year)
                    .ToList();
            }
        }
    }
}