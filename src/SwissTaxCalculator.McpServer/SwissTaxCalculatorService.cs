using System.Text.Json;
using Application.Tax.Contracts;
using Domain.Enums;
using Domain.Models.Municipality;
using Domain.Models.Tax;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace SwissTaxCalculator.McpServer;

/// <summary>
/// Swiss Tax Calculator Service that provides tax calculation logic
/// This is a service wrapper around IFullWealthAndIncomeTaxCalculator and IFullCapitalBenefitTaxCalculator
/// for the MCP server implementation
/// </summary>
public class SwissTaxCalculatorService(
    IFullWealthAndIncomeTaxCalculator wealthAndIncomeTaxCalculator,
    IFullCapitalBenefitTaxCalculator capitalBenefitTaxCalculator,
    ILogger<SwissTaxCalculatorService> logger)
{
    public async Task<string> CalculateWealthAndIncomeTax(
        int calculationYear,
        JsonElement municipality,
        JsonElement person,
        bool withMaxAvailableCalculationYear = false)
    {
        try
        {
            var municipalityModel = ParseMunicipality(municipality);
            var taxPerson = ParseTaxPerson(person);
            
            Either<string, FullTaxResult> result = await wealthAndIncomeTaxCalculator.CalculateAsync(
                calculationYear,
                municipalityModel,
                taxPerson,
                withMaxAvailableCalculationYear);

            return result.Match(
                Right: taxResult => JsonSerializer.Serialize(new
                {
                    success = true,
                    data = new
                    {
                        totalTaxAmount = taxResult.TotalTaxAmount,
                        stateTax = new
                        {
                            totalTaxAmount = taxResult.StateTaxResult.TotalTaxAmount,
                            totalIncomeTax = taxResult.StateTaxResult.TotalIncomeTax,
                            totalWealthTax = taxResult.StateTaxResult.TotalWealthTax,
                            cantonTaxAmount = taxResult.StateTaxResult.CantonTaxAmount,
                            municipalityTaxAmount = taxResult.StateTaxResult.MunicipalityTaxAmount,
                            churchTaxAmount = taxResult.StateTaxResult.ChurchTaxAmount,
                            pollTaxAmount = taxResult.StateTaxResult.PollTaxAmount,
                            cantonRate = taxResult.StateTaxResult.CantonRate,
                            municipalityRate = taxResult.StateTaxResult.MunicipalityRate
                        },
                        federalTax = new
                        {
                            taxAmount = taxResult.FederalTaxResult.TaxAmount,
                            determiningFactorTaxableAmount = taxResult.FederalTaxResult.DeterminingFactorTaxableAmount
                        }
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
            logger.LogError(ex, "Error in CalculateWealthAndIncomeTax");
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = ex.Message
            }, new JsonSerializerOptions { WriteIndented = true });
        }
    }

    public async Task<string> CalculateCapitalBenefitTax(
        int calculationYear,
        JsonElement municipality,
        JsonElement person,
        bool withMaxAvailableCalculationYear = false)
    {
        try
        {
            var municipalityModel = ParseMunicipality(municipality);
            var capitalBenefitPerson = ParseCapitalBenefitTaxPerson(person);
            
            Either<string, FullCapitalBenefitTaxResult> result = await capitalBenefitTaxCalculator.CalculateAsync(
                calculationYear,
                municipalityModel,
                capitalBenefitPerson,
                withMaxAvailableCalculationYear);

            return result.Match(
                Right: taxResult => JsonSerializer.Serialize(new
                {
                    success = true,
                    data = new
                    {
                        totalTaxAmount = taxResult.TotalTaxAmount,
                        stateResult = new
                        {
                            totalTaxAmount = taxResult.StateResult.TotalTaxAmount,
                            cantonTaxAmount = taxResult.StateResult.CantonTaxAmount,
                            municipalityTaxAmount = taxResult.StateResult.MunicipalityTaxAmount,
                            churchTaxAmount = taxResult.StateResult.ChurchTaxAmount,
                            cantonRate = taxResult.StateResult.CantonRate,
                            municipalityRate = taxResult.StateResult.MunicipalityRate
                        },
                        federalResult = new
                        {
                            taxAmount = taxResult.FederalResult.TaxAmount,
                            determiningFactorTaxableAmount = taxResult.FederalResult.DeterminingFactorTaxableAmount
                        }
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
            logger.LogError(ex, "Error in CalculateCapitalBenefitTax");
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = ex.Message
            }, new JsonSerializerOptions { WriteIndented = true });
        }
    }

    private static MunicipalityModel ParseMunicipality(JsonElement municipality)
    {
        return new MunicipalityModel
        {
            BfsNumber = municipality.GetProperty("bfsNumber").GetInt32(),
            Name = municipality.TryGetProperty("name", out var name) ? name.GetString() ?? "" : "",
            Canton = Enum.Parse<Canton>(municipality.GetProperty("canton").GetString() ?? "Undefined", true),
            EstvTaxLocationId = municipality.TryGetProperty("estvTaxLocationId", out var estvId) && !estvId.ValueKind.Equals(JsonValueKind.Null)
                ? estvId.GetInt32()
                : null
        };
    }

    private static TaxPerson ParseTaxPerson(JsonElement person)
    {
        return new TaxPerson
        {
            Name = person.TryGetProperty("name", out var name) ? name.GetString() ?? "" : "",
            CivilStatus = Enum.Parse<CivilStatus>(person.GetProperty("civilStatus").GetString() ?? "Undefined", true),
            NumberOfChildren = person.TryGetProperty("numberOfChildren", out var children) ? children.GetInt32() : 0,
            ReligiousGroupType = Enum.Parse<ReligiousGroupType>(
                person.TryGetProperty("religiousGroupType", out var religious) ? religious.GetString() ?? "Other" : "Other", true),
            PartnerReligiousGroupType = person.TryGetProperty("partnerReligiousGroupType", out var partnerReligious) && !partnerReligious.ValueKind.Equals(JsonValueKind.Null)
                ? Enum.Parse<ReligiousGroupType>(partnerReligious.GetString() ?? "Other", true)
                : null,
            TaxableIncome = person.GetProperty("taxableIncome").GetDecimal(),
            TaxableFederalIncome = person.GetProperty("taxableFederalIncome").GetDecimal(),
            TaxableWealth = person.GetProperty("taxableWealth").GetDecimal()
        };
    }

    private static CapitalBenefitTaxPerson ParseCapitalBenefitTaxPerson(JsonElement person)
    {
        return new CapitalBenefitTaxPerson
        {
            Name = person.TryGetProperty("name", out var name) ? name.GetString() ?? "" : "",
            CivilStatus = Enum.Parse<CivilStatus>(person.GetProperty("civilStatus").GetString() ?? "Undefined", true),
            NumberOfChildren = person.TryGetProperty("numberOfChildren", out var children) ? children.GetInt32() : 0,
            ReligiousGroupType = Enum.Parse<ReligiousGroupType>(
                person.TryGetProperty("religiousGroupType", out var religious) ? religious.GetString() ?? "Other" : "Other", true),
            PartnerReligiousGroupType = person.TryGetProperty("partnerReligiousGroupType", out var partnerReligious) && !partnerReligious.ValueKind.Equals(JsonValueKind.Null)
                ? Enum.Parse<ReligiousGroupType>(partnerReligious.GetString() ?? "Other", true)
                : null,
            TaxableCapitalBenefits = person.GetProperty("taxableCapitalBenefits").GetDecimal()
        };
    }
}
