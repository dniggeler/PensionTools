using Microsoft.Extensions.DependencyInjection;

namespace PensionCoach.Tools.PostOpenApi;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostOpenApiClient(this IServiceCollection services, string baseUrl)
    {
        services.AddHttpClient(PostOpenApiClient.ClientName, c =>
        {
            c.BaseAddress = new Uri(baseUrl);
        });

        services.AddTransient<IPostOpenApiClient, PostOpenApiClient>();

        return services;
    }
}
