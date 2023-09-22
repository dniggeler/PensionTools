using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.TaxComparison.Models;
using Domain.Enums;
using Domain.Models.MultiPeriod;
using Domain.Models.Tax;
using LanguageExt;
using PensionCoach.Tools.CommonTypes.MultiPeriod;

namespace BlazorApp.Services.Mock;

internal record PurchaseScenarioYears(
    int StartYearTransferIn,
    int EndYearTransferIn,
    int StartYearWithdrawal,
    int EndYearWithdrawal);

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

    public Task<CapitalBenefitsTransferInResponse> CalculateAsync(CapitalBenefitTransferInComparerRequest request)
    {
        PurchaseScenarioYears scenarioPeriod = GetPeriod();
        
        var benchmark = CreateBenchmarkResults().ToList();
        var scenario = CreateScenarioResults().ToList();

        var sumBenchmarkSeries =
            from w in benchmark.Where(item => item.AccountType == AccountType.Wealth)
            from p in benchmark.Where(item => item.AccountType == AccountType.OccupationalPension)
            where w.Year == p.Year
            select new { Sum = w.Amount + p.Amount, w.Year };

        var sumScenarioSeries =
            from w in scenario.Where(item => item.AccountType == AccountType.Wealth)
            from p in scenario.Where(item => item.AccountType == AccountType.OccupationalPension)
            where w.Year == p.Year
            select new { Sum = w.Amount + p.Amount, w.Year };

        IEnumerable<SinglePeriodCalculationResult> deltaResults = from bSum in sumBenchmarkSeries
                          from sSum in sumScenarioSeries
                          where bSum.Year == sSum.Year
                          select new SinglePeriodCalculationResult
                          {
                              Amount = sSum.Sum - bSum.Sum,
                              Year = bSum.Year,
                              AccountType = AccountType.Exogenous
                          };


        var result = new CapitalBenefitsTransferInResponse
        {
            NumberOfPeriods = scenarioPeriod.EndYearWithdrawal - scenarioPeriod.StartYearTransferIn + 1,
            StartingYear = scenarioPeriod.StartYearTransferIn,
            DeltaSeries = deltaResults,
            BenchmarkSeries = benchmark,
            ScenarioSeries = scenario
        };
        
        return result.AsTask();

        IEnumerable<SinglePeriodCalculationResult> CreateBenchmarkResults()
        {
            decimal currentWealth = decimal.Zero;
            decimal pensionCapital = decimal.Zero;

            int beginYear = scenarioPeriod.StartYearTransferIn;
            int endYear = scenarioPeriod.EndYearWithdrawal;

            for (int year = beginYear; year <= endYear; year++)
            {
                currentWealth *= (decimal.One + request.NetWealthReturn);

                if (year == request.CalculationYear)
                {
                    currentWealth += request.TaxableWealth;
                }

                if (request.WithCapitalBenefitTaxation)
                {
                    SingleTransferInModel withdrawal =
                        request.Withdrawals.FirstOrDefault(t => t.DateOfTransferIn.Year == year);

                    if (withdrawal is not null)
                    {
                        currentWealth += pensionCapital * withdrawal.Amount;
                        pensionCapital -= pensionCapital * withdrawal.Amount;
                    }
                }

                if (request.WithCapitalBenefitTaxation && scenarioPeriod.StartYearWithdrawal-1 == year)
                {
                    pensionCapital += request.CapitalBenefitsBeforeWithdrawal;
                }

                yield return new SinglePeriodCalculationResult
                {
                    Year = year,
                    AccountType = AccountType.Wealth,
                    Amount = currentWealth
                };

                yield return new SinglePeriodCalculationResult
                {
                    Year = year,
                    AccountType = AccountType.OccupationalPension,
                    Amount = pensionCapital
                };
            }
        }

        IEnumerable<SinglePeriodCalculationResult> CreateScenarioResults()
        {
            decimal currentWealth = decimal.Zero;
            decimal pensionCapital = decimal.Zero;

            int beginYear = scenarioPeriod.StartYearTransferIn;
            int endYear = scenarioPeriod.EndYearWithdrawal;

            for (int year = beginYear; year <= endYear; year++)
            {
                currentWealth *= (decimal.One + request.NetWealthReturn);

                if (year == request.CalculationYear)
                {
                    currentWealth += request.TaxableWealth;
                }

                SingleTransferInModel transfer = request.TransferIns.FirstOrDefault(t => t.DateOfTransferIn.Year == year);

                if (transfer is not null)
                {
                    currentWealth -= transfer.Amount;
                    pensionCapital += transfer.Amount;
                }

                if (request.WithCapitalBenefitTaxation)
                {
                    SingleTransferInModel withdrawal =
                        request.Withdrawals.FirstOrDefault(t => t.DateOfTransferIn.Year == year);

                    if (withdrawal is not null)
                    {
                        currentWealth += pensionCapital * withdrawal.Amount;
                        pensionCapital -= pensionCapital * withdrawal.Amount;
                    }
                }

                if (request.WithCapitalBenefitTaxation && scenarioPeriod.StartYearWithdrawal - 1 == year)
                {
                    pensionCapital += request.CapitalBenefitsBeforeWithdrawal;
                }

                yield return new SinglePeriodCalculationResult
                {
                    Year = year,
                    AccountType = AccountType.Wealth,
                    Amount = currentWealth
                };

                yield return new SinglePeriodCalculationResult
                {
                    Year = year,
                    AccountType = AccountType.OccupationalPension,
                    Amount = pensionCapital
                };
            }
        }

        PurchaseScenarioYears GetPeriod()
        {
            int beginOfPeriodTransferInYear = request.TransferIns.Min(t => t.DateOfTransferIn.Year);
            int endOfPeriodTransferInYear = request.TransferIns.Max(t => t.DateOfTransferIn.Year);

            int beginOfPeriodWithdrawalYear = endOfPeriodTransferInYear;
            int endOfPeriodWithdrawalYear = endOfPeriodTransferInYear;

            if (request.WithCapitalBenefitTaxation)
            {
                beginOfPeriodWithdrawalYear = request.Withdrawals.Min(t => t.DateOfTransferIn.Year);
                endOfPeriodWithdrawalYear = request.Withdrawals.Min(t => t.DateOfTransferIn.Year);
            }

            return new PurchaseScenarioYears(
                beginOfPeriodTransferInYear,
                endOfPeriodTransferInYear,
                beginOfPeriodWithdrawalYear,
                endOfPeriodWithdrawalYear);
        }
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
