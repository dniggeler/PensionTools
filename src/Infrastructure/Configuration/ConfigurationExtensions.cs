using Application.Enums;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Configuration;

public static class ConfigurationExtensions
{
    public static ApplicationMode GetApplicationMode(this IConfiguration configuration)
    {
        const string key = "ApplicationMode";

        return configuration[key] switch
        {
            null => ApplicationMode.Proprietary,
            { } v => Enum.Parse<ApplicationMode>(v),
        };
    }
}
