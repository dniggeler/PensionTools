using System;
using Microsoft.Extensions.Configuration;
using PensionCoach.Tools.CommonTypes.Tax;

namespace PensionCoach.Tools.CommonUtils;

public static class ConfigurationExtensions
{
    public static TypeOfTaxCalculator GetTypeOfTaxCalculator(this IConfiguration configuration)
    {
        const string key = "TaxCalculator:Provider";

        return configuration[key] switch
        {
            null => TypeOfTaxCalculator.PensionTools,
            {} v => Enum.Parse<TypeOfTaxCalculator>(v),
        };
    }
}
