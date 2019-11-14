using System;
using Microsoft.Extensions.DependencyInjection;
using Tax.Data.Abstractions;


namespace Tax.Data
{
    public static class TaxDataExtensions
    {
        public static void AddTaxData(this IServiceCollection collection)
        {
            collection.AddDbContext<FederalTaxTariffDbContext>(ServiceLifetime.Transient);
            collection.AddDbContext<TaxTariffDbContext>(ServiceLifetime.Transient);
            collection.AddDbContext<TaxRateDbContext>(ServiceLifetime.Transient);

            collection.AddSingleton<Func<TaxTariffDbContext>>(provider =>
                provider.GetRequiredService<TaxTariffDbContext>);
            collection.AddSingleton<Func<TaxRateDbContext>>(provider =>
                provider.GetRequiredService<TaxRateDbContext>);
            collection.AddSingleton<Func<FederalTaxTariffDbContext>>(provider =>
                provider.GetRequiredService<FederalTaxTariffDbContext>);

            collection.AddTransient<ITaxTariffData,TaxTariffData>();
        }
    }
}