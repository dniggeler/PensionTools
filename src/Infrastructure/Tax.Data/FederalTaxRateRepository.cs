using Application.Tax.Proprietary.Enums;
using Application.Tax.Proprietary.Repositories;
using Domain.Models.Tax;

namespace Infrastructure.Tax.Data
{
    public class FederalTaxRateRepository : IFederalTaxRateRepository
    {
        private readonly Func<FederalTaxTariffDbContext> federalDbContextFunc;

        public FederalTaxRateRepository(Func<FederalTaxTariffDbContext> federalDbContextFunc)
        {
            this.federalDbContextFunc = federalDbContextFunc;
        }

        public IEnumerable<FederalTaxTariffModel> TaxRates(int calculationYear, TariffType typeId)
        {
            using var dbContext = federalDbContextFunc();
            return dbContext.Tariffs
                .Where(item => item.Year == calculationYear)
                .Where(item => item.TariffType == (int)typeId)
                .ToList();
        }
    }
}
