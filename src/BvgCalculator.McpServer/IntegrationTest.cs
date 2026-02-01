using Application.Bvg;
using Application.Extensions;
using Domain.Enums;
using Domain.Models.Bvg;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BvgCalculator.McpServer;

/// <summary>
/// Simple test to verify the BVG Calculator integration works correctly
/// </summary>
public class BvgCalculatorIntegrationTest
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("BVG Calculator Integration Test");
        Console.WriteLine("================================\n");

        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        services.AddBvgCalculators();

        var serviceProvider = services.BuildServiceProvider();
        var calculator = serviceProvider.GetRequiredService<IBvgCalculator>();

        await RunTests(calculator);
    }

    private static async Task RunTests(IBvgCalculator calculator)
    {
        Console.WriteLine("Test 1: Basic BVG Calculation");
        Console.WriteLine("------------------------------");
        
        var person1 = new BvgPerson
        {
            DateOfBirth = new DateTime(1974, 3, 15),
            Gender = Gender.Male,
            ReportedSalary = 120_000m,
            PartTimeDegree = 1.0m,
            DisabilityDegree = 0.0m
        };

        var result1 = calculator.Calculate(2024, 350_000m, person1);
        
        result1.Match(
            Right: result =>
            {
                Console.WriteLine($"✓ Calculation successful");
                Console.WriteLine($"  Retirement Date: {result.DateOfRetirement:yyyy-MM-dd}");
                Console.WriteLine($"  Retirement Age: {result.RetirementAge.Years} years, {result.RetirementAge.Months} months");
                Console.WriteLine($"  Insured Salary: {result.InsuredSalary:N2} CHF");
                Console.WriteLine($"  Retirement Pension: {result.RetirementPension:N2} CHF/year");
                Console.WriteLine($"  Disability Pension: {result.DisabilityPension:N2} CHF/year");
                Console.WriteLine($"  Partner Pension: {result.PartnerPension:N2} CHF/year");
                return result;
            },
            Left: error =>
            {
                Console.WriteLine($"✗ Error: {error}");
                return null!;
            }
        );

        Console.WriteLine("\nTest 2: Insured Salary Calculation");
        Console.WriteLine("-----------------------------------");
        
        var person2 = new BvgPerson
        {
            DateOfBirth = new DateTime(1985, 7, 20),
            Gender = Gender.Female,
            ReportedSalary = 85_000m,
            PartTimeDegree = 1.0m,
            DisabilityDegree = 0.0m
        };

        var result2 = calculator.InsuredSalary(2024, person2);
        
        result2.Match(
            Right: salary =>
            {
                Console.WriteLine($"✓ Insured salary: {salary:N2} CHF");
                return salary;
            },
            Left: error =>
            {
                Console.WriteLine($"✗ Error: {error}");
                return 0m;
            }
        );

        Console.WriteLine("\nTest 3: Part-Time Worker");
        Console.WriteLine("------------------------");
        
        var person3 = new BvgPerson
        {
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = Gender.Female,
            ReportedSalary = 50_000m,
            PartTimeDegree = 0.6m,
            DisabilityDegree = 0.0m
        };

        var result3 = calculator.InsuredSalary(2024, person3);
        
        result3.Match(
            Right: salary =>
            {
                Console.WriteLine($"✓ Insured salary (60% part-time): {salary:N2} CHF");
                return salary;
            },
            Left: error =>
            {
                Console.WriteLine($"✗ Error: {error}");
                return 0m;
            }
        );

        Console.WriteLine("\nTest 4: Disability Scenario");
        Console.WriteLine("---------------------------");
        
        var person4 = new BvgPerson
        {
            DateOfBirth = new DateTime(1975, 6, 10),
            Gender = Gender.Male,
            ReportedSalary = 80_000m,
            PartTimeDegree = 1.0m,
            DisabilityDegree = 0.5m  // 50% disabled
        };

        var result4 = calculator.Calculate(2024, 300_000m, person4);
        
        result4.Match(
            Right: result =>
            {
                Console.WriteLine($"✓ Calculation with 50% disability successful");
                Console.WriteLine($"  Insured Salary: {result.InsuredSalary:N2} CHF");
                Console.WriteLine($"  Disability Pension: {result.DisabilityPension:N2} CHF/year");
                Console.WriteLine($"  Partner Pension: {result.PartnerPension:N2} CHF/year");
                return result;
            },
            Left: error =>
            {
                Console.WriteLine($"✗ Error: {error}");
                return null!;
            }
        );

        Console.WriteLine("\nTest 5: Time Series - Insured Salaries");
        Console.WriteLine("---------------------------------------");
        
        var person5 = new BvgPerson
        {
            DateOfBirth = new DateTime(1980, 1, 1),
            Gender = Gender.Male,
            ReportedSalary = 100_000m,
            PartTimeDegree = 1.0m,
            DisabilityDegree = 0.0m
        };

        var result5 = calculator.InsuredSalaries(2024, person5);
        
        result5.Match(
            Right: salaries =>
            {
                Console.WriteLine($"✓ Retrieved {salaries.Length} salary data points");
                Console.WriteLine($"  First entry: Age {salaries.First().Age.Years}y {salaries.First().Age.Months}m - {salaries.First().Value:N2} CHF");
                Console.WriteLine($"  Last entry: Age {salaries.Last().Age.Years}y {salaries.Last().Age.Months}m - {salaries.Last().Value:N2} CHF");
                return salaries;
            },
            Left: error =>
            {
                Console.WriteLine($"✗ Error: {error}");
                return Array.Empty<Application.Bvg.Models.BvgTimeSeriesPoint>();
            }
        );

        Console.WriteLine("\n================================");
        Console.WriteLine("All tests completed!");
        Console.WriteLine("================================");
    }
}
