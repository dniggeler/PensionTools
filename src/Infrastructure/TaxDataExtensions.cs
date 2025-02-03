using Application.Municipality;
using Infrastructure.DataStaging;
using Infrastructure.Municipality;
using Infrastructure.Tax.Data;
using Infrastructure.Tax.Data.Populate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class TaxDataExtensions
{
    public static void AddTaxData(this IServiceCollection collection, IConfiguration configuration)
    {
        var connectionString = "Filename=" + configuration.GetConnectionString("TaxDb");

        collection.AddTransient(_ =>
        {
            var opt = new DbContextOptionsBuilder<MunicipalityDbContext>();
            opt.UseSqlite(connectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            return opt.Options;
        });

        collection.AddDbContext<MunicipalityDbContext>(opt => opt.UseSqlite(connectionString), ServiceLifetime.Transient);

        collection.AddTransient<IMunicipalityRepository, MunicipalityRepository>();
        collection.AddTransient<ITaxDataPopulateService, StaticTaxDataPopulateService>();
    }
}
