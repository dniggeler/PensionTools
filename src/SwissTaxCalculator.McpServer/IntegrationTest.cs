using System.Text.Json;
using Application.Tax.Contracts;
using Domain.Enums;
using Domain.Models.Municipality;
using Domain.Models.Tax;
using LanguageExt;
using Microsoft.Extensions.Logging.Abstractions;

namespace SwissTaxCalculator.McpServer;

/// <summary>
/// Integration test examples for the Swiss Tax Calculator MCP Server
/// These tests demonstrate how to use the service programmatically
/// </summary>
public class IntegrationTest
{
    public static async Task RunWealthAndIncomeTaxExample()
    {
        // This is a mock implementation for demonstration purposes
        var mockCalculator = new MockWealthAndIncomeTaxCalculator();
        var service = new SwissTaxCalculatorService(
            mockCalculator,
            new MockCapitalBenefitTaxCalculator(),
            NullLogger<SwissTaxCalculatorService>.Instance);

        var municipalityJson = JsonSerializer.SerializeToElement(new
        {
            bfsNumber = 261,
            canton = "ZH",
            name = "Zurich"
        });

        var personJson = JsonSerializer.SerializeToElement(new
        {
            civilStatus = "Married",
            numberOfChildren = 2,
            religiousGroupType = "Protestant",
            taxableIncome = 100000,
            taxableFederalIncome = 100000,
            taxableWealth = 500000
        });

        var result = await service.CalculateWealthAndIncomeTax(
            2024,
            municipalityJson,
            personJson,
            false);

        Console.WriteLine("Wealth and Income Tax Result:");
        Console.WriteLine(result);
    }

    public static async Task RunCapitalBenefitTaxExample()
    {
        var mockCalculator = new MockCapitalBenefitTaxCalculator();
        var service = new SwissTaxCalculatorService(
            new MockWealthAndIncomeTaxCalculator(),
            mockCalculator,
            NullLogger<SwissTaxCalculatorService>.Instance);

        var municipalityJson = JsonSerializer.SerializeToElement(new
        {
            bfsNumber = 351,
            canton = "BE",
            name = "Bern"
        });

        var personJson = JsonSerializer.SerializeToElement(new
        {
            civilStatus = "Single",
            religiousGroupType = "Catholic",
            taxableCapitalBenefits = 200000
        });

        var result = await service.CalculateCapitalBenefitTax(
            2024,
            municipalityJson,
            personJson,
            false);

        Console.WriteLine("Capital Benefit Tax Result:");
        Console.WriteLine(result);
    }
}

/// <summary>
/// Mock implementation for testing purposes
/// </summary>
internal class MockWealthAndIncomeTaxCalculator : IFullWealthAndIncomeTaxCalculator
{
    public Task<Either<string, FullTaxResult>> CalculateAsync(
        int calculationYear,
        MunicipalityModel municipality,
        TaxPerson person,
        bool withMaxAvailableCalculationYear = false)
    {
        var result = new FullTaxResult
        {
            StateTaxResult = new StateTaxResult
            {
                BasisIncomeTax = new Application.Tax.Proprietary.Abstractions.Models.BasisTaxResult
                {
                    TaxAmount = 5000,
                    DeterminingFactorTaxableAmount = 100000
                },
                BasisWealthTax = new Application.Tax.Proprietary.Abstractions.Models.BasisTaxResult
                {
                    TaxAmount = 1000,
                    DeterminingFactorTaxableAmount = 500000
                },
                ChurchTax = new ChurchTaxResult
                {
                    TaxAmount = 500
                },
                CantonRate = 100,
                MunicipalityRate = 50,
                PollTaxAmount = 0
            },
            FederalTaxResult = new Application.Tax.Proprietary.Abstractions.Models.BasisTaxResult
            {
                TaxAmount = 4500,
                DeterminingFactorTaxableAmount = 100000
            }
        };

        return Task.FromResult<Either<string, FullTaxResult>>(result);
    }
}

/// <summary>
/// Mock implementation for testing purposes
/// </summary>
internal class MockCapitalBenefitTaxCalculator : IFullCapitalBenefitTaxCalculator
{
    public Task<Either<string, FullCapitalBenefitTaxResult>> CalculateAsync(
        int calculationYear,
        MunicipalityModel municipality,
        CapitalBenefitTaxPerson person,
        bool withMaxAvailableCalculationYear = false)
    {
        var result = new FullCapitalBenefitTaxResult
        {
            StateResult = new CapitalBenefitTaxResult
            {
                BasisTax = new Application.Tax.Proprietary.Abstractions.Models.BasisTaxResult
                {
                    TaxAmount = 15000,
                    DeterminingFactorTaxableAmount = 200000
                },
                ChurchTax = new ChurchTaxResult
                {
                    TaxAmount = 1000
                },
                CantonRate = 100,
                MunicipalityRate = 40
            },
            FederalResult = new Application.Tax.Proprietary.Abstractions.Models.BasisTaxResult
            {
                TaxAmount = 13000,
                DeterminingFactorTaxableAmount = 200000
            }
        };

        return Task.FromResult<Either<string, FullCapitalBenefitTaxResult>>(result);
    }
}
