using Microsoft.Extensions.DependencyInjection;

namespace Tax.Data
{
    public static class TaxDataExtensions
    {
        public static void AddTaxData(this ServiceCollection collection)
        {
            collection.AddDbContext<TaxTariffDbContext>();
            collection.AddDbContext<TaxRateDbContext>();
        }
    }
}