using Application.Features.FullTaxCalculation;
using Application.Tax;
using Application.Tax.Proprietary.Abstractions.Models;
using Domain.Enums;
using Domain.Models.Tax;
using LanguageExt;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infrastructure.HealthChecks
{
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
}
