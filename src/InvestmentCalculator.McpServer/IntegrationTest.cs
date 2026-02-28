using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace InvestmentCalculator.McpServer.Tests;

/// <summary>
/// Integration tests for the Investment Calculator MCP Server
/// These tests verify the server responds correctly to MCP protocol messages
/// </summary>
public class InvestmentCalculatorIntegrationTests
{
    private const string ProjectPath = "../../../InvestmentCalculator.McpServer.csproj";

    /// <summary>
    /// Test that the server initializes correctly
    /// </summary>
    public static async Task TestInitialize()
    {
        var (stdout, stderr) = await RunServer(
            @"{""jsonrpc"":""2.0"",""id"":1,""method"":""initialize"",""params"":{""protocolVersion"":""2024-11-05"",""capabilities"":{},""clientInfo"":{""name"":""test"",""version"":""1.0""}}}"
        );

        var response = JsonDocument.Parse(stdout);
        var result = response.RootElement.GetProperty("result");
        
        Console.WriteLine("✓ Initialize test passed");
        Console.WriteLine($"  Protocol Version: {result.GetProperty("protocolVersion").GetString()}");
        Console.WriteLine($"  Server Name: {result.GetProperty("serverInfo").GetProperty("name").GetString()}");
    }

    /// <summary>
    /// Test that tools/list returns the financial_projection tool
    /// </summary>
    public static async Task TestToolsList()
    {
        var (stdout, stderr) = await RunServer(
            @"{""jsonrpc"":""2.0"",""id"":2,""method"":""tools/list"",""params"":{}}"
        );

        var response = JsonDocument.Parse(stdout);
        var tools = response.RootElement.GetProperty("result").GetProperty("tools");
        
        Console.WriteLine("✓ Tools list test passed");
        Console.WriteLine($"  Number of tools: {tools.GetArrayLength()}");
        
        foreach (var tool in tools.EnumerateArray())
        {
            Console.WriteLine($"  - {tool.GetProperty("name").GetString()}: {tool.GetProperty("description").GetString()}");
        }
    }

    /// <summary>
    /// Test a basic financial projection calculation
    /// </summary>
    public static async Task TestFinancialProjection()
    {
        var request = new
        {
            jsonrpc = "2.0",
            id = 3,
            method = "tools/call",
            @params = new
            {
                name = "financial_projection",
                arguments = new
                {
                    projectionInterestRate = 0.015,
                    dateOfRetirement = "2040-12-31",
                    dateOfEndOfSavings = "2040-12-31",
                    retirementAge = new { years = 65, months = 0 },
                    finalAge = new { years = 67, months = 0 },
                    yearOfBeginProjection = 2040,
                    beginOfRetirementCapital = 500000,
                    retirementCredits = new Dictionary<string, decimal>
                    {
                        ["64_0"] = 14000,
                        ["65_0"] = 0,
                        ["66_0"] = 0
                    }
                }
            }
        };

        var requestJson = JsonSerializer.Serialize(request);
        var (stdout, stderr) = await RunServer(requestJson);

        var response = JsonDocument.Parse(stdout);
        var result = response.RootElement.GetProperty("result");
        var content = result.GetProperty("content")[0].GetProperty("text").GetString()!;
        var calculationResult = JsonDocument.Parse(content);

        if (calculationResult.RootElement.GetProperty("success").GetBoolean())
        {
            var data = calculationResult.RootElement.GetProperty("data");
            var projectionTable = data.GetProperty("projectionTable");
            var totalRows = data.GetProperty("totalRows").GetInt32();
            var finalCapital = data.GetProperty("finalCapital").GetDecimal();

            Console.WriteLine("✓ Financial projection test passed");
            Console.WriteLine($"  Total rows: {totalRows}");
            Console.WriteLine($"  Start date: {data.GetProperty("startDate").GetString()}");
            Console.WriteLine($"  End date: {data.GetProperty("endDate").GetString()}");
            Console.WriteLine($"  Final capital: {finalCapital:N2} CHF");
            
            // Show first and last entry
            var firstEntry = projectionTable[0];
            Console.WriteLine($"  First entry date: {firstEntry.GetProperty("dateOfCalculation").GetString()}");
            Console.WriteLine($"  First entry capital: {firstEntry.GetProperty("retirementCapital").GetDecimal():N2}");
            
            var lastIndex = projectionTable.GetArrayLength() - 1;
            var lastEntry = projectionTable[lastIndex];
            Console.WriteLine($"  Last entry date: {lastEntry.GetProperty("dateOfCalculation").GetString()}");
            Console.WriteLine($"  Last entry capital: {lastEntry.GetProperty("retirementCapital").GetDecimal():N2}");
        }
        else
        {
            var error = calculationResult.RootElement.GetProperty("error").GetString();
            Console.WriteLine($"✗ Financial projection test failed: {error}");
        }
    }

    /// <summary>
    /// Test error handling with invalid parameters
    /// </summary>
    public static async Task TestErrorHandling()
    {
        var request = new
        {
            jsonrpc = "2.0",
            id = 4,
            method = "tools/call",
            @params = new
            {
                name = "financial_projection",
                arguments = new
                {
                    // Missing required parameters
                    projectionInterestRate = 0.015
                }
            }
        };

        var requestJson = JsonSerializer.Serialize(request);
        var (stdout, stderr) = await RunServer(requestJson);

        var response = JsonDocument.Parse(stdout);
        var result = response.RootElement.GetProperty("result");
        var content = result.GetProperty("content")[0].GetProperty("text").GetString()!;
        var calculationResult = JsonDocument.Parse(content);

        if (!calculationResult.RootElement.GetProperty("success").GetBoolean())
        {
            Console.WriteLine("✓ Error handling test passed");
            Console.WriteLine($"  Error message: {calculationResult.RootElement.GetProperty("error").GetString()}");
        }
        else
        {
            Console.WriteLine("✗ Error handling test failed: Expected error but got success");
        }
    }

    private static async Task<(string stdout, string stderr)> RunServer(string input)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project {ProjectPath}",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        var stdoutBuilder = new StringBuilder();
        var stderrBuilder = new StringBuilder();

        process.OutputDataReceived += (s, e) =>
        {
            if (e.Data != null)
                stdoutBuilder.AppendLine(e.Data);
        };

        process.ErrorDataReceived += (s, e) =>
        {
            if (e.Data != null)
                stderrBuilder.AppendLine(e.Data);
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.StandardInput.WriteLineAsync(input);
        await process.StandardInput.FlushAsync();

        // Give it a moment to process
        await Task.Delay(1000);

        if (!process.HasExited)
        {
            process.Kill(true);
        }

        await process.WaitForExitAsync();

        return (stdoutBuilder.ToString().Trim(), stderrBuilder.ToString().Trim());
    }

    /// <summary>
    /// Run all integration tests
    /// </summary>
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Investment Calculator MCP Server - Integration Tests");
        Console.WriteLine("=====================================================\n");

        try
        {
            await TestInitialize();
            Console.WriteLine();

            await TestToolsList();
            Console.WriteLine();

            await TestFinancialProjection();
            Console.WriteLine();

            await TestErrorHandling();
            Console.WriteLine();

            Console.WriteLine("=====================================================");
            Console.WriteLine("All tests completed!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Test suite failed with exception: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}
