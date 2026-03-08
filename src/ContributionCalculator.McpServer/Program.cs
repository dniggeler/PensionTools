using System.Text.Json;
using Application.Features.ContributionCalculator;
using ContributionCalculator.McpServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

// Add logging to file instead of console (which would interfere with MCP stdio)
services.AddLogging(builder =>
{
    builder.SetMinimumLevel(LogLevel.Error);
});

// Register Contribution Calculator
services.AddTransient<ICompoundingContributionCalculator, Application.Features.ContributionCalculator.CompoundingContributionCalculator>();
services.AddSingleton<ContributionCalculatorService>();

// Build service provider
var serviceProvider = services.BuildServiceProvider();

// Get the service
var calculatorService = serviceProvider.GetRequiredService<ContributionCalculatorService>();

// Simple MCP stdio handler
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (s, e) =>
{
    cts.Cancel();
    e.Cancel = true;
};

try
{
    while (!cts.Token.IsCancellationRequested)
    {
        var line = await Console.In.ReadLineAsync(cts.Token);
        if (line == null) break;

        try
        {
            var request = JsonDocument.Parse(line);
            var root = request.RootElement;
            
            if (root.TryGetProperty("method", out var methodProp))
            {
                var method = methodProp.GetString();
                
                if (method == "initialize")
                {
                    // Respond to initialize
                    var response = JsonSerializer.Serialize(new
                    {
                        jsonrpc = "2.0",
                        id = root.GetProperty("id").GetInt32(),
                        result = new
                        {
                            protocolVersion = "2024-11-05",
                            capabilities = new
                            {
                                tools = new { }
                            },
                            serverInfo = new
                            {
                                name = "contribution-calculator-server",
                                version = "1.0.0"
                            }
                        }
                    });
                    await Console.Out.WriteLineAsync(response);
                }
                else if (method == "tools/list")
                {
                    var response = JsonSerializer.Serialize(new
                    {
                        jsonrpc = "2.0",
                        id = root.GetProperty("id").GetInt32(),
                        result = new
                        {
                            tools = new object[]
                            {
                                new
                                {
                                    name = "calculate_contribution_projection",
                                    description = "Calculate the future value of a series of contributions (irregular dates and amounts) plus an initial investment, given an annual interest rate (30/360 day count convention) and a final date.",
                                    inputSchema = new
                                    {
                                        type = "object",
                                        properties = new
                                        {
                                            startDate = new { type = "string", description = "Start date of the projection (ISO format)" },
                                            initialAmount = new { type = "number", description = "Initial investment amount at start date" },
                                            contributions = new 
                                            { 
                                                type = "array", 
                                                items = new 
                                                { 
                                                    type = "object", 
                                                    properties = new 
                                                    { 
                                                        date = new { type = "string" }, 
                                                        amount = new { type = "number" } 
                                                    },
                                                    required = new[] { "date", "amount" }
                                                },
                                                description = "List of contributions with date and amount"
                                            },
                                            annualInterestRate = new { type = "number", description = "Annual interest rate (decimal, e.g. 0.05 for 5%)" },
                                            finalDate = new { type = "string", description = "Final date of the projection (ISO format)" },
                                            compoundingFrequency = new 
                                            { 
                                                type = "string", 
                                                @enum = new[] { "Annual", "SemiAnnual", "Quarterly", "Monthly", "Daily", "Continuous" },
                                                description = "Compounding frequency (default: Annual)"
                                            },
                                            returnSequence = new { type = "boolean", description = "If true, returns the sequence of balances at each contribution date" }
                                        },
                                        required = new[] { "startDate", "initialAmount", "annualInterestRate", "finalDate" }
                                    }
                                },
                                new
                                {
                                    name = "calculate_fixed_yearly_contribution",
                                    description = "Calculate a projection with a fixed yearly contribution amount for a fixed number of contributions.",
                                    inputSchema = new
                                    {
                                        type = "object",
                                        properties = new
                                        {
                                            startDate = new { type = "string", description = "Start date of the projection (ISO format)" },
                                            initialAmount = new { type = "number", description = "Initial investment amount at start date" },
                                            yearlyContributionAmount = new { type = "number", description = "Yearly contribution amount" },
                                            contributionCount = new { type = "integer", description = "Number of yearly contributions" },
                                            investAtBeginningOfYear = new { type = "boolean", description = "If true, contributions are invested at the start of each year" },
                                            annualInterestRate = new { type = "number", description = "Annual interest rate (decimal, e.g. 0.05 for 5%)" },
                                            compoundingFrequency = new
                                            {
                                                type = "string",
                                                @enum = new[] { "Annual", "SemiAnnual", "Quarterly", "Monthly", "Daily", "Continuous" },
                                                description = "Compounding frequency"
                                            },
                                            returnSequence = new { type = "boolean", description = "If true, returns the sequence of balances" }
                                        },
                                        required = new[]
                                        {
                                            "startDate",
                                            "initialAmount",
                                            "yearlyContributionAmount",
                                            "contributionCount",
                                            "investAtBeginningOfYear",
                                            "annualInterestRate",
                                            "compoundingFrequency"
                                        }
                                    }
                                },
                                new
                                {
                                    name = "calculate_yearly_contribution_until_final_date",
                                    description = "Calculate a projection with fixed yearly contributions until a final date.",
                                    inputSchema = new
                                    {
                                        type = "object",
                                        properties = new
                                        {
                                            startDate = new { type = "string", description = "Start date of the projection (ISO format)" },
                                            finalDate = new { type = "string", description = "Final date of the projection (ISO format)" },
                                            initialAmount = new { type = "number", description = "Initial investment amount at start date" },
                                            yearlyContributionAmount = new { type = "number", description = "Yearly contribution amount" },
                                            investAtBeginningOfYear = new { type = "boolean", description = "If true, contributions are invested at the start of each year" },
                                            annualInterestRate = new { type = "number", description = "Annual interest rate (decimal, e.g. 0.05 for 5%)" },
                                            compoundingFrequency = new
                                            {
                                                type = "string",
                                                @enum = new[] { "Annual", "SemiAnnual", "Quarterly", "Monthly", "Daily", "Continuous" },
                                                description = "Compounding frequency"
                                            },
                                            returnSequence = new { type = "boolean", description = "If true, returns the sequence of balances" }
                                        },
                                        required = new[]
                                        {
                                            "startDate",
                                            "finalDate",
                                            "initialAmount",
                                            "yearlyContributionAmount",
                                            "investAtBeginningOfYear",
                                            "annualInterestRate",
                                            "compoundingFrequency"
                                        }
                                    }
                                }
                            }
                        }
                    });
                    await Console.Out.WriteLineAsync(response);
                }
                else if (method == "tools/call")
                {
                    var paramsEl = root.GetProperty("params");
                    var toolName = paramsEl.GetProperty("name").GetString();
                    var arguments = paramsEl.GetProperty("arguments");

                    string resultText = toolName switch
                    {
                        "calculate_contribution_projection" => calculatorService.CalculateContribution(arguments),
                        "calculate_fixed_yearly_contribution" => calculatorService.CalculateFixedYearlyContribution(arguments),
                        "calculate_yearly_contribution_until_final_date" => calculatorService.CalculateYearlyContributionUntilFinalDate(arguments),
                        _ => JsonSerializer.Serialize(new
                        {
                            success = false,
                            error = $"Unknown tool: {toolName}"
                        })
                    };

                    var response = JsonSerializer.Serialize(new
                    {
                        jsonrpc = "2.0",
                        id = root.GetProperty("id").GetInt32(),
                        result = new
                        {
                            content = new object[]
                            {
                                new
                                {
                                    type = "text",
                                    text = resultText
                                }
                            }
                        }
                    });
                    await Console.Out.WriteLineAsync(response);
                }
            }
        }
        catch (Exception)
        {
            // Try to report error
            // Check if we can parse id?
            // Just write to stderr or ignore if we can't reply
        }
    }
}
catch (OperationCanceledException)
{
    // Normal shutdown
}

return 0;
