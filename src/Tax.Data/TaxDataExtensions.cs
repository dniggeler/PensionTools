using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tax.Data.Abstractions;


namespace Tax.Data
{
    public static class TaxDataExtensions
    {
        public static void AddTaxData(
            this IServiceCollection collection, IConfiguration configuration)
        {
            var connectionString = "Filename="+configuration.GetConnectionString("TaxDb");

            collection.AddDbContext<FederalTaxTariffDbContext>(
                opt => opt.UseSqlite(connectionString), ServiceLifetime.Transient);
            collection.AddDbContext<TaxTariffDbContext>(
                opt => opt.UseSqlite(connectionString), ServiceLifetime.Transient);
            collection.AddDbContext<TaxRateDbContext>(
                opt => opt.UseSqlite(connectionString), ServiceLifetime.Transient);

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