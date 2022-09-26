using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxComparison;

namespace BlazorApp.Services.Mock;

public class MockCapitalBenefitsComparisonService : ITaxCapitalBenefitsComparisonService
{
    private readonly Random randomGenerator;

    public MockCapitalBenefitsComparisonService()
    {
        this.randomGenerator = new Random();
    }

    public async IAsyncEnumerable<CapitalBenefitTaxComparerResponse> CalculateAsync(CapitalBenefitTaxComparerRequest request)
    {
        int numberOfSamples = 200;
        string[] municipalityNames = { "Bagnes", "Bern", "Zürich", "Lachen", "Wettingen", "Zuzwil" };

        for (int ii = 0; ii < numberOfSamples; ii++)
        {
            string randomName = municipalityNames[randomGenerator.Next(0, municipalityNames.Length - 1)];
            string[] cantonNames = Enum.GetNames(typeof(Canton));
            Canton randomCanton = Enum.Parse<Canton>(cantonNames[randomGenerator.Next(0, cantonNames.Length - 1)]);
            int randomId = randomGenerator.Next(137, 9000);
            decimal randomMunicipalityTax = GetRandomAmount(60000,90000);
            decimal randomChurchTax = GetRandomAmount(100, 3000);
            decimal federalTax = request.TaxableBenefits / 20M;
            decimal randomCantonTax = Math.Max(0, randomMunicipalityTax + GetRandomAmount(-5000, 5000));

            decimal totalTax = randomCantonTax + randomMunicipalityTax + randomChurchTax + federalTax;

            yield return new CapitalBenefitTaxComparerResponse
            {
                MunicipalityName = randomName,
                MunicipalityId = randomId,
                Canton = randomCanton,
                MaxSupportedTaxYear = 2022,
                Name = $"Mock {ii+1}",
                TotalTaxAmount = totalTax,
                TaxDetails = new TaxAmountDetail
                {
                    CantonTaxAmount = randomCantonTax,
                    FederalTaxAmount = federalTax,
                    MunicipalityTaxAmount = randomMunicipalityTax,
                    ChurchTaxAmount = randomChurchTax
                },
                CountComputed = ii+1,
                TotalCount = numberOfSamples
            };
        }

        yield return new CapitalBenefitTaxComparerResponse
        {
            MunicipalityName = "Langnau aA",
            MunicipalityId = 136,
            Canton = Canton.ZH,
            MaxSupportedTaxYear = 2022,
            Name = "Mock Langnau",
            TotalTaxAmount = 115_500,
            TaxDetails = new TaxAmountDetail
            {
                CantonTaxAmount = 45_000,
                FederalTaxAmount = 20_000,
                MunicipalityTaxAmount = 50_000,
                ChurchTaxAmount = 500
            },
            CountComputed = numberOfSamples + 1,
            TotalCount = numberOfSamples
        };

        decimal GetRandomAmount(decimal min, decimal max)
        {
            return min + (max - min) * (decimal)randomGenerator.NextDouble();
        }
    }
}
