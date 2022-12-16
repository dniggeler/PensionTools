using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
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
        const decimal marginalTaxRate = 0.3M;
        const decimal marginalCapitalBenefitTaxRate = 0.08M;
        int defaultYear = 2025;

        int beginOfPeriodYear = request.TransferIns.Min(t => t.DateOfTransferIn.Year);
        int endOfPeriodYear = request.TransferIns.Max(t => t.DateOfTransferIn.Year);

        if (request.WithCapitalBenefitTaxation)
        {
            endOfPeriodYear = request.YearOfCapitalBenefitWithdrawal ?? defaultYear;
        }

        List<SinglePeriodCalculationResult> singleResults = new();
        for (int year = beginOfPeriodYear; year <= endOfPeriodYear; year++)
        {
            var transferIn = request.TransferIns.FirstOrDefault(t => t.DateOfTransferIn.Year == year);

            if (transferIn is null)
            {
                continue;
            }

            SinglePeriodCalculationResult singleResultWealth = new SinglePeriodCalculationResult
            {
                AccountType = AccountType.Wealth,
                Year = year,
                Amount = -transferIn.Amount * (decimal.One - marginalTaxRate)
            };

            SinglePeriodCalculationResult singleResultPensionAccount = new SinglePeriodCalculationResult
            {
                AccountType = AccountType.OccupationalPension,
                Year = year,
                Amount = transferIn.Amount
            };

            singleResults.Add(singleResultWealth);
            singleResults.Add(singleResultPensionAccount);
        }

        if (request.WithCapitalBenefitTaxation)
        {
            decimal transferInTotal = request.TransferIns.Sum(t => t.Amount);
            decimal capitalBenefitTaxAmount = transferInTotal * marginalCapitalBenefitTaxRate;

            var existingResult = singleResults.SingleOrDefault(t => t.Year == endOfPeriodYear);

            if (existingResult is null)
            {
                SinglePeriodCalculationResult taxResult = new SinglePeriodCalculationResult
                {
                    AccountType = AccountType.Wealth,
                    Year = endOfPeriodYear,
                    Amount = -capitalBenefitTaxAmount
                };
                
                singleResults.Add(taxResult);
            }
            else
            {
                existingResult.Amount -= capitalBenefitTaxAmount;
            }
            
        }
        
        var result = new MultiPeriodResponse
        {
            NumberOfPeriods = endOfPeriodYear - beginOfPeriodYear + 1,
            StartingYear = beginOfPeriodYear,
            Accounts = singleResults
        };

        return result.AsTask();
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
