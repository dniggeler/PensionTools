using Application.Features.CheckSettings;
using Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DataStaging;

public static class DataStagingServiceCollectionExtensions
{
    public static void AddDataStagingServices(this IServiceCollection collection)
    {
        collection.AddTransient<IDataStagingConnector, DataStagingConnector>();
        collection.AddTransient<ICheckSettingsConnector, CheckSettingsConnector>();

    }
}
