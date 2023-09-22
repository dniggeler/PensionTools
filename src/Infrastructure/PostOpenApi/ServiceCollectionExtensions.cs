using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.PostOpenApi
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPostOpenApiClient(this IServiceCollection services, IConfiguration configuration)
        {
            string baseUrl = configuration["TaxCalculatorClient:PostOpenApiBaseUrl"];

            services.AddHttpClient(PostOpenApiClient.ClientName, c =>
            {
                c.BaseAddress = new Uri(baseUrl);
            });

            services.AddTransient<IPostOpenApiClient, PostOpenApiClient>();

            return services;
        }
    }
}
