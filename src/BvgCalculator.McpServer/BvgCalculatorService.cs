using System.Text.Json;
using Application.Bvg;
using Application.Bvg.Models;
using Domain.Enums;
using Domain.Models.Bvg;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace BvgCalculator.McpServer;

/// <summary>
/// BVG Calculator Service that provides calculation logic
/// This is a simple service wrapper around IBvgCalculator
/// for the MCP server implementation
/// </summary>
public class BvgCalculatorService(IBvgCalculator bvgCalculator, ILogger<BvgCalculatorService> logger)
{
    public string Calculate(int calculationYear, decimal retirementCapitalEndOfYear, JsonElement person)
    {
        try
        {
            BvgPerson bvgPerson = ParseBvgPerson(person);
            Either<string, BvgCalculationResult> result = bvgCalculator.Calculate(calculationYear, retirementCapitalEndOfYear, bvgPerson);

            return result.Match(
                Right: calculationResult => JsonSerializer.Serialize(new
                {
                    success = true,
                    data = new
                    {
                        dateOfRetirement = calculationResult.DateOfRetirement,
                        retirementAge = new
                        {
                            years = calculationResult.RetirementAge.Years,
                            months = calculationResult.RetirementAge.Months
                        },
                        effectiveSalary = calculationResult.EffectiveSalary,
                        insuredSalary = calculationResult.InsuredSalary,
                        retirementCredit = calculationResult.RetirementCredit,
                        retirementCreditFactor = calculationResult.RetirementCreditFactor,
                        retirementCapitalEndOfYear = calculationResult.RetirementCapitalEndOfYear,
                        finalRetirementCapital = calculationResult.FinalRetirementCapital,
                        finalRetirementCapitalWithoutInterest = calculationResult.FinalRetirementCapitalWithoutInterest,
                        retirementPension = calculationResult.RetirementPension,
                        disabilityPension = calculationResult.DisabilityPension,
                        partnerPension = calculationResult.PartnerPension,
                        orphanPension = calculationResult.OrphanPension,
                        childPensionForDisabled = calculationResult.ChildPensionForDisabled
                    }
                }, new JsonSerializerOptions { WriteIndented = true }),
                Left: error => JsonSerializer.Serialize(new
                {
                    success = false,
                    error
                }, new JsonSerializerOptions { WriteIndented = true })
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in Calculate");
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = ex.Message
            }, new JsonSerializerOptions { WriteIndented = true });
        }
    }

    public string InsuredSalary(int calculationYear, JsonElement person)
    {
        try
        {
            BvgPerson bvgPerson = ParseBvgPerson(person);
            Either<string, decimal> result = bvgCalculator.InsuredSalary(calculationYear, bvgPerson);

            return result.Match(
                Right: salary => JsonSerializer.Serialize(new
                {
                    success = true,
                    data = new
                    {
                        insuredSalary = salary,
                        calculationYear
                    }
                }, new JsonSerializerOptions { WriteIndented = true }),
                Left: error => JsonSerializer.Serialize(new
                {
                    success = false,
                    error
                }, new JsonSerializerOptions { WriteIndented = true })
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in InsuredSalary");
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = ex.Message
            }, new JsonSerializerOptions { WriteIndented = true });
        }
    }

    public string InsuredSalariesTimeSeries(int calculationYear, JsonElement person)
    {
        try
        {
            BvgPerson bvgPerson = ParseBvgPerson(person);
            Either<string, BvgTimeSeriesPoint[]> result = bvgCalculator.InsuredSalaries(calculationYear, bvgPerson);

            return result.Match(
                Right: salaries => JsonSerializer.Serialize(new
                {
                    success = true,
                    data = new
                    {
                        timeSeries = salaries.Select(s => new
                        {
                            date = s.Date,
                            age = new { years = s.Age.Years, months = s.Age.Months },
                            value = s.Value
                        }).ToArray()
                    }
                }, new JsonSerializerOptions { WriteIndented = true }),
                Left: error => JsonSerializer.Serialize(new
                {
                    success = false,
                    error
                }, new JsonSerializerOptions { WriteIndented = true })
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in InsuredSalariesTimeSeries");
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = ex.Message
            }, new JsonSerializerOptions { WriteIndented = true });
        }
    }

    public string RetirementCreditFactors(int calculationYear, JsonElement person)
    {
        try
        {
            BvgPerson bvgPerson = ParseBvgPerson(person);
            Either<string, BvgTimeSeriesPoint[]> result = bvgCalculator.RetirementCreditFactors(calculationYear, bvgPerson);

            return result.Match(
                Right: factors => JsonSerializer.Serialize(new
                {
                    success = true,
                    data = new
                    {
                        timeSeries = factors.Select(f => new
                        {
                            date = f.Date,
                            age = new { years = f.Age.Years, months = f.Age.Months },
                            factor = f.Value
                        }).ToArray()
                    }
                }, new JsonSerializerOptions { WriteIndented = true }),
                Left: error => JsonSerializer.Serialize(new
                {
                    success = false,
                    error
                }, new JsonSerializerOptions { WriteIndented = true })
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in RetirementCreditFactors");
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = ex.Message
            }, new JsonSerializerOptions { WriteIndented = true });
        }
    }

    public string RetirementCredits(int calculationYear, JsonElement person)
    {
        try
        {
            BvgPerson bvgPerson = ParseBvgPerson(person);
            Either<string, BvgTimeSeriesPoint[]> result = bvgCalculator.RetirementCredits(calculationYear, bvgPerson);

            return result.Match(
                Right: credits => JsonSerializer.Serialize(new
                {
                    success = true,
                    data = new
                    {
                        timeSeries = credits.Select(c => new
                        {
                            date = c.Date,
                            age = new { years = c.Age.Years, months = c.Age.Months },
                            credit = c.Value
                        }).ToArray()
                    }
                }, new JsonSerializerOptions { WriteIndented = true }),
                Left: error => JsonSerializer.Serialize(new
                {
                    success = false,
                    error
                }, new JsonSerializerOptions { WriteIndented = true })
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in RetirementCredits");
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = ex.Message
            }, new JsonSerializerOptions { WriteIndented = true });
        }
    }

    private BvgPerson ParseBvgPerson(JsonElement personElement)
    {
        return new BvgPerson
        {
            DateOfBirth = DateTime.Parse(personElement.GetProperty("dateOfBirth").GetString() ?? throw new ArgumentException("dateOfBirth is required")),
            //Gender = Enum.Parse<Gender>(personElement.GetProperty("gender").GetString() ?? throw new ArgumentException("gender is required")),
            Gender = Gender.Male,
            ReportedSalary = personElement.GetProperty("reportedSalary").GetDecimal(),
            PartTimeDegree = personElement.TryGetProperty("partTimeDegree", out JsonElement ptd) ? ptd.GetDecimal() : 1.0m,
            DisabilityDegree = personElement.TryGetProperty("disabilityDegree", out JsonElement dd) ? dd.GetDecimal() : 0.0m
        };
    }
}
