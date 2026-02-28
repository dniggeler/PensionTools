using System.Text.Json;
using Application.Features.FullTaxCalculation;
using Domain.Enums;
using Infrastructure.EstvTaxCalculator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SwissTaxCalculator.McpServer;

var services = new ServiceCollection();

// Add logging
services.AddLogging(builder =>
{
    builder.SetMinimumLevel(LogLevel.Error);
});

// Register Tax Calculator services with Mock mode for now
// Change to ApplicationMode.Estv if you have ESTV API access configured
services.AddTaxCalculators(ApplicationMode.Estv);

// Register Tax Calculator Service
services.AddSingleton<SwissTaxCalculatorService>();
services.AddEstvTaxCalculatorClient("https://swisstaxcalculator.estv.admin.ch/delegate/ost-integration/v1/lg-proxy/operation/c3b67379_ESTV/", false);

// Build service provider
var serviceProvider = services.BuildServiceProvider();

// Get the service
var calculatorService = serviceProvider.GetRequiredService<SwissTaxCalculatorService>();

// Simple MCP stdio handler
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
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
                                name = "swiss-tax-calculator-server",
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
                                    name = "calculate_wealth_and_income_tax",
                                    description = "Calculate Swiss wealth and income tax for a person in a specific municipality",
                                    inputSchema = new
                                    {
                                        type = "object",
                                        properties = new
                                        {
                                            calculationYear = new { type = "integer", description = "The tax year for calculation" },
                                            taxLocationId = new { type = "integer", description = "The ESTV tax location id" },
                                            person = new
                                            {
                                                type = "object",
                                                properties = new
                                                {
                                                    name = new { type = "string", description = "Person's name (optional)" },
                                                    civilStatus = new { type = "string", description = "Civil status: 'Single' or 'Married'" },
                                                    numberOfChildren = new { type = "integer", description = "Number of children (default: 0)" },
                                                    religiousGroupType = new { type = "string", description = "Religious group: 'Other', 'Protestant', 'Catholic', 'Roman' (default: 'Other')" },
                                                    partnerReligiousGroupType = new { type = "string", description = "Partner's religious group (optional, for married persons)" },
                                                    taxableIncome = new { type = "number", description = "Taxable income amount" },
                                                    taxableFederalIncome = new { type = "number", description = "Taxable federal income amount" },
                                                    taxableWealth = new { type = "number", description = "Taxable wealth amount" }
                                                },
                                                required = new[] { "civilStatus", "taxableIncome", "taxableFederalIncome", "taxableWealth" }
                                            }
                                        },
                                        required = new[] { "calculationYear", "taxLocationId", "person" }
                                    }
                                },
                                new
                                {
                                    name = "calculate_capital_benefit_tax",
                                    description = "Calculate Swiss capital benefit tax (e.g., pension lump sum withdrawal) for a person in a specific municipality",
                                    inputSchema = new
                                    {
                                        type = "object",
                                        properties = new
                                        {
                                            calculationYear = new { type = "integer", description = "The tax year for calculation" },
                                            taxLocationId = new { type = "integer", description = "The ESTV tax location id" },
                                            person = new
                                            {
                                                type = "object",
                                                properties = new
                                                {
                                                    name = new { type = "string", description = "Person's name (optional)" },
                                                    civilStatus = new { type = "string", description = "Civil status: 'Single' or 'Married'" },
                                                    numberOfChildren = new { type = "integer", description = "Number of children (default: 0)" },
                                                    religiousGroupType = new { type = "string", description = "Religious group: 'Other', 'Protestant', 'Catholic', 'Roman' (default: 'Other')" },
                                                    partnerReligiousGroupType = new { type = "string", description = "Partner's religious group (optional, for married persons)" },
                                                    taxableCapitalBenefits = new { type = "number", description = "Taxable capital benefits amount (e.g., pension lump sum)" }
                                                },
                                                required = new[] { "civilStatus", "taxableCapitalBenefits" }
                                            }
                                        },
                                        required = new[] { "calculationYear", "taxLocationId", "person" }
                                    }
                                },
                                new
                                {
                                    name = "search_municipalities",
                                    description = "Search for Swiss municipalities by name, canton, and year of validity",
                                    inputSchema = new
                                    {
                                        type = "object",
                                        properties = new
                                        {
                                            searchFilter = new
                                            {
                                                type = "object",
                                                properties = new
                                                {
                                                    canton = new { type = "string", description = "Canton short name (e.g., 'ZH', 'BE', 'GE'). Optional." },
                                                    name = new { type = "string", description = "Municipality name (substring search). Optional." },
                                                    yearOfValidity = new { type = "integer", description = "Year of validity for municipality data. Optional." }
                                                }
                                            }
                                        },
                                        required = new[] { "searchFilter" }
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
                        "calculate_wealth_and_income_tax" => await calculatorService.CalculateWealthAndIncomeTax(
                            arguments.GetProperty("calculationYear").GetInt32(),
                            arguments.GetProperty("taxLocationId").GetInt32(),
                            arguments.GetProperty("person")),
                        "calculate_capital_benefit_tax" => await calculatorService.CalculateCapitalBenefitTax(
                            arguments.GetProperty("calculationYear").GetInt32(),
                            arguments.GetProperty("taxLocationId").GetInt32(),
                            arguments.GetProperty("person")),
                        "search_municipalities" => await calculatorService.SearchMunicipalities(
                            arguments.GetProperty("searchFilter")),
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
