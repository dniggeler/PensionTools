using System.Text.Json;
using Application.Features.ContributionCalculator;
using Application.Features.ContributionCalculator.Models;
using Microsoft.Extensions.Logging;

namespace ContributionCalculator.McpServer;

public class ContributionCalculatorService(
    ICompoundingContributionCalculator calculator,
    ILogger<ContributionCalculatorService> logger)
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    public string CalculateContribution(JsonElement parameters)
    {
        try
        {
            var request = JsonSerializer.Deserialize<ContributionRequest>(parameters.GetRawText(), SerializerOptions);

            if (request == null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = "Invalid request parameters"
                });
            }

            var resultEither = calculator.Calculate(request);

            return resultEither.Match(
                Right: result => JsonSerializer.Serialize(new
                {
                    success = true,
                    data = result
                }, new JsonSerializerOptions { WriteIndented = true }),
                Left: error => JsonSerializer.Serialize(new
                {
                    success = false,
                    error = error
                })
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in CalculateContribution");
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = ex.Message,
                stackTrace = ex.StackTrace
            }, new JsonSerializerOptions { WriteIndented = true });
        }
    }

    public string CalculateFixedYearlyContribution(JsonElement parameters)
    {
        try
        {
            var request = JsonSerializer.Deserialize<FixedYearlyContributionRequest>(parameters.GetRawText(), SerializerOptions);

            if (request == null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = "Invalid request parameters"
                });
            }

            var resultEither = calculator.CalculateFixedYearlyContribution(
                request.StartDate,
                request.InitialAmount,
                request.YearlyContributionAmount,
                request.ContributionCount,
                request.InvestAtBeginningOfYear,
                request.AnnualInterestRate,
                request.CompoundingFrequency,
                request.ReturnSequence);

            return resultEither.Match(
                Right: result => JsonSerializer.Serialize(new
                {
                    success = true,
                    data = result
                }, new JsonSerializerOptions { WriteIndented = true }),
                Left: error => JsonSerializer.Serialize(new
                {
                    success = false,
                    error = error
                })
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in CalculateFixedYearlyContribution");
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = ex.Message,
                stackTrace = ex.StackTrace
            }, new JsonSerializerOptions { WriteIndented = true });
        }
    }

    public string CalculateYearlyContributionUntilFinalDate(JsonElement parameters)
    {
        try
        {
            var request = JsonSerializer.Deserialize<YearlyContributionUntilFinalDateRequest>(parameters.GetRawText(), SerializerOptions);

            if (request == null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    error = "Invalid request parameters"
                });
            }

            var resultEither = calculator.CalculateYearlyContributionUntilFinalDate(
                request.StartDate,
                request.FinalDate,
                request.InitialAmount,
                request.YearlyContributionAmount,
                request.InvestAtBeginningOfYear,
                request.AnnualInterestRate,
                request.CompoundingFrequency,
                request.ReturnSequence);

            return resultEither.Match(
                Right: result => JsonSerializer.Serialize(new
                {
                    success = true,
                    data = result
                }, new JsonSerializerOptions { WriteIndented = true }),
                Left: error => JsonSerializer.Serialize(new
                {
                    success = false,
                    error = error
                })
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in CalculateYearlyContributionUntilFinalDate");
            return JsonSerializer.Serialize(new
            {
                success = false,
                error = ex.Message,
                stackTrace = ex.StackTrace
            }, new JsonSerializerOptions { WriteIndented = true });
        }
    }

    private record FixedYearlyContributionRequest(
        DateTime StartDate,
        decimal InitialAmount,
        decimal YearlyContributionAmount,
        int ContributionCount,
        bool InvestAtBeginningOfYear,
        decimal AnnualInterestRate,
        CompoundingFrequency CompoundingFrequency,
        bool ReturnSequence = false);

    private record YearlyContributionUntilFinalDateRequest(
        DateTime StartDate,
        DateTime FinalDate,
        decimal InitialAmount,
        decimal YearlyContributionAmount,
        bool InvestAtBeginningOfYear,
        decimal AnnualInterestRate,
        CompoundingFrequency CompoundingFrequency,
        bool ReturnSequence = false);
}
