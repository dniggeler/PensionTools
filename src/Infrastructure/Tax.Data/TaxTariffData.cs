using Application.Tax.Proprietary.Abstractions.Repositories;
using Domain.Models.Tax;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tax.Data
{
    public class TaxTariffData : ITaxTariffRepository
    {
        private readonly Func<TaxTariffDbContext> dbContext;

        public TaxTariffData(Func<TaxTariffDbContext> dbContext)
        {
            this.dbContext = dbContext;
        }

        public IReadOnlyCollection<TaxTariffModel> Get(TaxFilterModel filter)
        {
            using var ctxt = dbContext();
            List<TaxTariffModel> tariffs = ctxt.Tariffs.AsNoTracking()
                .Where(item => item.Canton == filter.Canton)
                .ToList();

            int maxYear = Math.Min(filter.Year, tariffs.Max(item => item.Year));

            return tariffs
                .Where(item => item.Year == maxYear)
                .ToList();
        }
    }
}
