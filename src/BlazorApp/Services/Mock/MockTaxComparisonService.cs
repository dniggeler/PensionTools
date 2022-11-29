using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.MultiPeriod;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxComparison;

namespace BlazorApp.Services.Mock;

public class MockTaxComparisonService : ITaxComparisonService, ITaxScenarioService
{
    const int NumberOfSamples = 200;

    private readonly Random randomGenerator;
    readonly string[] municipalityNames = { "Bagnes", "Bern", "Zürich", "Lachen", "Wettingen", "Zuzwil" };

    public MockTaxComparisonService()
    {
        this.randomGenerator = new Random();
    }

    public async IAsyncEnumerable<TaxComparerResponse> CalculateAsync(CapitalBenefitTaxComparerRequest request)
    {
        await foreach (var result in CalculateRandomlyAsync(request.TaxableBenefits))
        {
            yield return result;
        }
    }

    public async IAsyncEnumerable<TaxComparerResponse> CalculateAsync(IncomeAndWealthComparerRequest request)
    {
        await foreach (var result in CalculateRandomlyAsync(request.TaxableFederalIncome))
        {
            yield return result;
        }
    }

    public Task<MultiPeriodResponse> CalculateAsync(CapitalBenefitTransferInComparerRequest request)
    {
        throw new NotImplementedException();
    }

    private async IAsyncEnumerable<TaxComparerResponse> CalculateRandomlyAsync(decimal anchorAmount)
    {

        for (int ii = 0; ii < NumberOfSamples; ii++)
        {
            string randomName = municipalityNames[randomGenerator.Next(0, municipalityNames.Length - 1)];
            string[] cantonNames = Enum.GetNames(typeof(Canton));
            Canton randomCanton = Enum.Parse<Canton>(cantonNames[randomGenerator.Next(0, cantonNames.Length - 1)]);
            int randomId = randomGenerator.Next(137, 9000);
            decimal randomMunicipalityTax = Math.Max(anchorAmount - 15000 + GetRandomAmount(0, 30000), 0);
            decimal randomChurchTax = GetRandomAmount(100, 3000);
            decimal federalTax = anchorAmount / 20M;
            decimal randomCantonTax = Math.Max(0, randomMunicipalityTax + GetRandomAmount(-5000, 5000));

            decimal totalTax = randomCantonTax + randomMunicipalityTax + randomChurchTax + federalTax;

            yield return new TaxComparerResponse
            {
                MunicipalityName = randomName,
                MunicipalityId = randomId,
                Canton = randomCanton,
                MaxSupportedTaxYear = 2022,
                Name = $"Mock {ii + 1}",
                TotalTaxAmount = totalTax,
                TaxDetails = new TaxAmountDetail
                {
                    CantonTaxAmount = randomCantonTax,
                    FederalTaxAmount = federalTax,
                    MunicipalityTaxAmount = randomMunicipalityTax,
                    ChurchTaxAmount = randomChurchTax
                },
                CountComputed = ii + 1,
                TotalCount = NumberOfSamples
            };
        }

        yield return new TaxComparerResponse
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
            CountComputed = NumberOfSamples + 1,
            TotalCount = NumberOfSamples
        };

        decimal GetRandomAmount(decimal min, decimal max)
        {
            return min + (max - min) * (decimal)randomGenerator.NextDouble();
        }
    }

}
