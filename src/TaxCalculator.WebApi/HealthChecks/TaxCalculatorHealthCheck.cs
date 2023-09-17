using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Enums;
using LanguageExt;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace TaxCalculator.WebApi.HealthChecks;

public class TaxCalculatorHealthCheck : IHealthCheck
{
    private readonly ITaxCalculatorConnector taxCalculatorConnector;

    public TaxCalculatorHealthCheck(ITaxCalculatorConnector taxCalculatorConnector)
    {
        this.taxCalculatorConnector = taxCalculatorConnector;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
    {
        try
        {
            Either<string, FullTaxResult> result = await taxCalculatorConnector.CalculateAsync(2023, 261,
                new TaxPerson()
                {
                    CivilStatus = CivilStatus.Single,
                    Name = "HealthCheck",
                    NumberOfChildren = 0,
                    TaxableIncome = 100_000,
                    TaxableWealth = 0,
                    TaxableFederalIncome = 100_000,
                    ReligiousGroupType = ReligiousGroupType.Other,
                });

            return result.Match(
                Right: _ => HealthCheckResult.Healthy("Tax calculator ok"),
                Left: error => HealthCheckResult.Unhealthy($"Tax calculator nok {error}"));
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"An unhealthy result: {ex.Message}");
        }
    }
}
