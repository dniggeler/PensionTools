using System.Text.Json;
using Application.Extensions;
using BvgCalculator.McpServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

// Add logging to file instead of console (which would interfere with MCP stdio)
services.AddLogging(builder =>
{
    builder.SetMinimumLevel(LogLevel.Error);
});

// Register BVG Calculator services
services.AddBvgCalculators();

// Register BVG Calculator Service
services.AddSingleton<BvgCalculatorService>();

// Build service provider
var serviceProvider = services.BuildServiceProvider();

// Get the service
var calculatorService = serviceProvider.GetRequiredService<BvgCalculatorService>();

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
                                name = "bvg-calculator-server",
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
                                    name = "bvg_calculate",
                                    description = "Berechnet die gesetzlich einzuhaltenden BVG-Leistungen (schweizerische berufliche Vorsorge)",
                                    inputSchema = new
                                    {
                                        type = "object",
                                        properties = new
                                        {
                                            calculationYear = new { type = "integer" },
                                            retirementCapitalEndOfYear = new { type = "number" },
                                            person = new
                                            {
                                                type = "object",
                                                properties = new
                                                {
                                                    dateOfBirth = new { type = "string" },
                                                    gender = new { type = "string" },
                                                    reportedSalary = new { type = "number" }
                                                },
                                                required = new[] { "dateOfBirth", "gender", "reportedSalary" }
                                            }
                                        },
                                        required = new[] { "calculationYear", "retirementCapitalEndOfYear", "person" }
                                    }
                                },
                                new
                                {
                                    name = "bvg_insured_salary",
                                    description = "Berechnet das BVG-versicherte Einkommen",
                                    inputSchema = new
                                    {
                                        type = "object",
                                        properties = new
                                        {
                                            calculationYear = new { type = "integer" },
                                            person = new
                                            {
                                                type = "object",
                                                properties = new
                                                {
                                                    dateOfBirth = new { type = "string" },
                                                    gender = new { type = "string" },
                                                    reportedSalary = new { type = "number" }
                                                },
                                                required = new[] { "dateOfBirth", "gender", "reportedSalary" }
                                            }
                                        },
                                        required = new[] { "calculationYear", "person" }
                                    }
                                },
                                new
                                {
                                    name = "bvg_retirement_date",
                                    description = "Berechnet das offizielle BVG-Rentenalter und Rentendatum einer Person",
                                    inputSchema = new
                                    {
                                        type = "object",
                                        properties = new
                                        {
                                            person = new
                                            {
                                                type = "object",
                                                properties = new
                                                {
                                                    dateOfBirth = new { type = "string" },
                                                    gender = new { type = "string" }
                                                },
                                                required = new[] { "dateOfBirth", "gender" }
                                            }
                                        },
                                        required = new[] { "person" }
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
                        "bvg_calculate" => calculatorService.Calculate(
                            arguments.GetProperty("calculationYear").GetInt32(),
                            arguments.GetProperty("retirementCapitalEndOfYear").GetDecimal(),
                            arguments.GetProperty("person")),
                        "bvg_insured_salary" => calculatorService.InsuredSalary(
                            arguments.GetProperty("calculationYear").GetInt32(),
                            arguments.GetProperty("person")),
                        "bvg_insured_salaries_timeseries" => calculatorService.InsuredSalariesTimeSeries(
                            arguments.GetProperty("calculationYear").GetInt32(),
                            arguments.GetProperty("person")),
                        "bvg_retirement_credit_factors" => calculatorService.RetirementCreditFactors(
                            arguments.GetProperty("calculationYear").GetInt32(),
                            arguments.GetProperty("person")),
                        "bvg_retirement_credits" => calculatorService.RetirementCredits(
                            arguments.GetProperty("calculationYear").GetInt32(),
                            arguments.GetProperty("person")),
                        "bvg_retirement_date" => calculatorService.RetirementDate(
                            arguments.GetProperty("person")),
                        _ => JsonSerializer.Serialize(new { success = false, error = "Unknown tool" })
                    };

                    var response = JsonSerializer.Serialize(new
                    {
                        jsonrpc = "2.0",
                        id = root.GetProperty("id").GetInt32(),
                        result = new
                        {
                            content = new[]
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
