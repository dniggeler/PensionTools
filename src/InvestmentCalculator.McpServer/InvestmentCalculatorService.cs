using System.Text.Json;
using Application.Bvg;
using Domain.Models.Bvg;
using Microsoft.Extensions.Logging;

namespace InvestmentCalculator.McpServer;

/// <summary>
/// Investment Calculator Service that provides financial projection calculation
/// This service wrapper exposes ISavingsProcessProjectionCalculator
/// for the MCP server implementation
/// </summary>
public class InvestmentCalculatorService(
    ISavingsProcessProjectionCalculator projectionCalculator,
    ILogger<InvestmentCalculatorService> logger)
{
    public string CompoundInterestProjection(JsonElement parameters)
    {
        try
        {
            var annualNetInterestRate = parameters.GetProperty("annualNetInterestRate").GetDecimal();
            var dateOfRetirement = DateTime.Parse(parameters.GetProperty("dateOfRetirement").GetString()!);
            
            var retirementAgeEl = parameters.GetProperty("retirementAge");
            var retirementAge = TechnicalAge.From(
                retirementAgeEl.GetProperty("years").GetInt32(),
                retirementAgeEl.GetProperty("months").GetInt32()
            );
            
            var finalAge = retirementAge;
            
            var yearOfBeginProjection = parameters.GetProperty("yearOfBeginProjection").GetInt32();
            var initialInvestmentAmount = parameters.GetProperty("initialInvestmentAmount").GetDecimal();
            
            // Create retirement credit getter function that returns fixed investment amount
            Func<TechnicalAge, decimal> investmentAmountGetter = _ => decimal.Zero;

            var results = projectionCalculator.ProjectionTable(
                annualNetInterestRate,
                dateOfRetirement,
                dateOfRetirement,
                retirementAge,
                retirementAge,
                yearOfBeginProjection,
                initialInvestmentAmount,
                investmentAmountGetter
            );

            var projectionData = results
                .Where(r => r.IsFullYear || r.IsEndOfSavings)
                .Select(r => new
            {
                dateOfCalculation = r.DateOfCalculation.ToString("yyyy-MM-dd"),
                bvgAge = r.BvgAge,
                technicalAge = new
                {
                    years = r.TechnicalAge.Years,
                    months = r.TechnicalAge.Months
                },
                proRatedFactor = r.ProRatedFactor,
                grossInterestRate = r.GrossInterestRate,
                retirementCredit = r.RetirementCredit,
                retirementCapitalWithoutInterest = r.RetirementCapitalWithoutInterest,
                retirementCapital = r.RetirementCapital,
                isRetirementDate = r.IsRetirementDate,
                isEndOfSavings = r.IsEndOfSavings,
                isFullYear = r.IsFullYear,
                isFullAge = r.IsFullAge
            }).ToArray();

            return JsonSerializer.Serialize(new
            {
                success = true,
                data = new
                {
                    projectionTable = projectionData,
                    totalRows = projectionData.Length,
                    startDate = projectionData.FirstOrDefault()?.dateOfCalculation,
                    endDate = projectionData.LastOrDefault()?.dateOfCalculation,
                    finalCapital = projectionData.LastOrDefault()?.retirementCapital
                }
            }, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in CalculateProjection");
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = ex.Message,
                stackTrace = ex.StackTrace
            }, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
