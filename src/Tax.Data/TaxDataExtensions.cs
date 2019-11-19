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

            collection.AddDbContext<FederalTaxTariffDbContext>(opt => opt.UseSqlite(connectionString));
            collection.AddDbContext<TaxTariffDbContext>(opt => opt.UseSqlite(connectionString));
            collection.AddDbContext<TaxRateDbContext>(opt => opt.UseSqlite(connectionString));

            collection.AddSingleton<Func<TaxTariffDbContext>>(provider =>
                provider.GetRequiredService<TaxTariffDbContext>);
            collection.AddSingleton<Func<TaxRateDbContext>>(provider =>
                provider.GetRequiredService<TaxRateDbContext>);
            collection.AddSingleton<Func<FederalTaxTariffDbContext>>(provider =>
                provider.GetRequiredService<FederalTaxTariffDbContext>);

            collection.AddScoped<ITaxTariffData,TaxTariffData>();
        }
    }
}