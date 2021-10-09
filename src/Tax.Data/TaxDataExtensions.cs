using System;
using System.IO;
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

            collection.AddTransient(ctxt =>
            {
                var opt = new DbContextOptionsBuilder<FederalTaxTariffDbContext>();
                opt.UseSqlite(connectionString)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

                return opt.Options;
            });

            collection.AddTransient(ctxt =>
            {
                var opt = new DbContextOptionsBuilder<MunicipalityDbContext>();
                opt.UseSqlite(connectionString)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

                return opt.Options;
            });

            collection.AddTransient(ctxt =>
            {
                var opt = new DbContextOptionsBuilder<TaxRateDbContext>();
                opt.UseSqlite(connectionString)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

                return opt.Options;
            });

            collection.AddTransient(ctxt =>
            {
                var opt = new DbContextOptionsBuilder<TaxTariffDbContext>();
                opt.UseSqlite(connectionString)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

                return opt.Options;
            });

            collection.AddDbContext<FederalTaxTariffDbContext>(
                opt => opt.UseSqlite(connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking),
                ServiceLifetime.Transient);

            collection.AddDbContext<TaxTariffDbContext>(
                opt => opt.UseSqlite(connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking),
                ServiceLifetime.Transient);

            collection.AddDbContext<TaxRateDbContext>(
                opt => opt.UseSqlite(connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking),
                ServiceLifetime.Transient);

            collection.AddDbContext<MunicipalityDbContext>(
                opt => opt.UseSqlite(connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking),
                ServiceLifetime.Transient);

            collection.AddSingleton<Func<TaxTariffDbContext>>(provider => provider.GetRequiredService<TaxTariffDbContext>);
            collection.AddSingleton<Func<TaxRateDbContext>>(provider => provider.GetRequiredService<TaxRateDbContext>);
            collection.AddSingleton<Func<FederalTaxTariffDbContext>>(provider =>provider.GetRequiredService<FederalTaxTariffDbContext>);
            collection.AddSingleton<Func<MunicipalityDbContext>>(provider => provider.GetRequiredService<MunicipalityDbContext>);

            collection.AddSingleton<ITaxTariffData,TaxTariffData>();
        }
    }
}
