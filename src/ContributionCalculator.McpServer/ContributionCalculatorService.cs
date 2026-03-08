using System.Text.Json;
using Application.Features.ContributionCalculator;
using Application.Features.ContributionCalculator.Models;
using Microsoft.Extensions.Logging;

namespace ContributionCalculator.McpServer;

public class ContributionCalculatorService(
    ICompoundingContributionCalculator calculator,
    ILogger<ContributionCalculatorService> logger)
{
    public string CalculateContribution(JsonElement parameters)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            };

            var request = JsonSerializer.Deserialize<ContributionRequest>(parameters.GetRawText(), options);

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
}
