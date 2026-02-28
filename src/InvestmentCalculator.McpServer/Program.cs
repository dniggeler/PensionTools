using System.Text.Json;
using Application.Extensions;
using InvestmentCalculator.McpServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

// Add logging to file instead of console (which would interfere with MCP stdio)
services.AddLogging(builder =>
{
    builder.SetMinimumLevel(LogLevel.Error);
});

// Register BVG Calculator services (includes ISavingsProcessProjectionCalculator)
services.AddBvgCalculators();

// Register Investment Calculator Service
services.AddSingleton<InvestmentCalculatorService>();

// Build service provider
var serviceProvider = services.BuildServiceProvider();

// Get the service
var calculatorService = serviceProvider.GetRequiredService<InvestmentCalculatorService>();

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
                                name = "investment-calculator-server",
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
                                    name = "financial_projection",
                                    description = "Calculate a multi-period compound interest financial projection showing year-by-year growth of an initial investment amount. Returns a detailed breakdown of the investment value for each year from the start of the projection until the specified retirement date, applying the given annual net interest rate.",
                                    inputSchema = new
                                    {
                                        type = "object",
                                        properties = new
                                        {
                                            annualNetInterestRate = new 
                                            { 
                                                type = "number",
                                                description = "Annual net interest rate for projection (e.g., 0.02 for 2%)"
                                            },
                                            dateOfRetirement = new 
                                            { 
                                                type = "string",
                                                description = "Date of retirement in ISO format (YYYY-MM-DD)"
                                            },
                                            retirementAge = new
                                            {
                                                type = "object",
                                                properties = new
                                                {
                                                    years = new { type = "integer" },
                                                    months = new { type = "integer" }
                                                },
                                                required = new[] { "years", "months" }
                                            },
                                            yearOfBeginProjection = new 
                                            { 
                                                type = "integer",
                                                description = "Starting year for the projection"
                                            },
                                            initialInvestmentAmount = new 
                                            { 
                                                type = "number",
                                                description = "Initial investment amount at the beginning of the projection"
                                            }
                                        },
                                        required = new[] 
                                        { 
                                            "annualNetInterestRate", 
                                            "dateOfRetirement", 
                                            "retirementAge",
                                            "yearOfBeginProjection",
                                            "initialInvestmentAmount"
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
                        "financial_projection" => calculatorService.CompoundInterestProjection(arguments),
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
        catch (Exception ex)
        {
            var errorResponse = JsonSerializer.Serialize(new
            {
                jsonrpc = "2.0",
                error = new
                {
                    code = -32603,
                    message = ex.Message
                }
            });
            await Console.Out.WriteLineAsync(errorResponse);
        }
    }
}
catch (OperationCanceledException)
{
    // Normal shutdown
}

return 0;
