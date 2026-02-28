# Investment Calculator MCP Server

A Model Context Protocol (MCP) server implementation that exposes financial projection calculation functionality for retirement savings and investments.

## Overview

This MCP server provides access to investment and savings projection calculations through standardized tools that can be consumed by MCP clients. It implements the MCP protocol over stdio (standard input/output), exposing the `ISavingsProcessProjectionCalculator` interface as an MCP tool.

## Implementation

The server uses a simple stdio-based MCP protocol implementation that:
- Reads JSON-RPC requests from stdin
- Processes tool calls using the Investment Calculator
- Writes JSON-RPC responses to stdout

This approach ensures maximum compatibility with all MCP clients.

## Available Tools

### 1. `financial_projection`
Calculate a detailed financial projection for retirement savings over time, including interest compounding and retirement credits.

**Parameters:**
- `projectionInterestRate` (number): Annual interest rate for projection (e.g., 0.02 for 2%)
- `dateOfRetirement` (string): Date of retirement in ISO format (YYYY-MM-DD)
- `dateOfEndOfSavings` (string): Date when savings contributions end in ISO format (YYYY-MM-DD)
- `retirementAge` (object): Technical retirement age
  - `years` (integer): Years component
  - `months` (integer): Months component (0-11)
- `finalAge` (object): Final age for projection
  - `years` (integer): Years component
  - `months` (integer): Months component (0-11)
- `yearOfBeginProjection` (integer): Starting year for the projection
- `beginOfRetirementCapital` (number): Initial retirement capital at the beginning
- `retirementCredits` (object): Dictionary of retirement credits by technical age
  - Format: `"years_months": amount` (e.g., `"45_0": 15000` for 45 years, 0 months)

**Returns:**
A detailed projection table with monthly calculations including:
- Date of calculation
- BVG age and technical age
- Pro-rated factor
- Gross interest rate
- Retirement credit for the period
- Retirement capital with and without interest
- Flags for retirement date, end of savings, full year, and full age
- Summary with total rows, date range, and final capital

**Example:**
```json
{
  "projectionInterestRate": 0.015,
  "dateOfRetirement": "2040-12-31",
  "dateOfEndOfSavings": "2040-12-31",
  "retirementAge": {
    "years": 65,
    "months": 0
  },
  "finalAge": {
    "years": 70,
    "months": 0
  },
  "yearOfBeginProjection": 2025,
  "beginOfRetirementCapital": 500000,
  "retirementCredits": {
    "40_0": 12000,
    "41_0": 12500,
    "42_0": 13000,
    "43_0": 13500,
    "44_0": 14000,
    "45_0": 14500
  }
}
```

## Building and Running

### Prerequisites
- .NET 10 SDK
- Access to the official MCP .NET SDK (ModelContextProtocol NuGet package)

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run
```

The server will start and connect via stdio (standard input/output), which is the standard MCP transport mechanism.

## Configuration with MCP Clients

To use this server with an MCP client (e.g., Claude Desktop), add the following to your MCP configuration:

```json
{
  "mcpServers": {
    "investment-calculator": {
      "command": "dotnet",
      "args": ["run", "--project", "path/to/InvestmentCalculator.McpServer/InvestmentCalculator.McpServer.csproj"]
    }
  }
}
```

Or if using the published executable:

```json
{
  "mcpServers": {
    "investment-calculator": {
      "command": "path/to/InvestmentCalculator.McpServer.exe"
    }
  }
}
```

### For Claude Desktop on Windows:
The configuration file is typically located at:
```
%APPDATA%\Claude\claude_desktop_config.json
```

### For Claude Desktop on macOS:
```
~/Library/Application Support/Claude/claude_desktop_config.json
```

## Use Cases

This calculator is useful for:
- Retirement planning and savings projections
- Swiss BVG (occupational pension) projections
- Investment growth calculations with periodic contributions
- Financial planning scenarios with variable contribution rates
- Long-term savings analysis with compound interest

## Architecture

The server follows a clean architecture pattern:
- **Program.cs**: MCP protocol handler (stdio-based JSON-RPC)
- **InvestmentCalculatorService.cs**: Service layer that wraps the calculator
- **ISavingsProcessProjectionCalculator**: Core calculation interface from Application layer
- **SingleSavingsProcessProjectionCalculator**: Implementation with detailed month-by-month projections

## Testing

To test the server manually, you can send JSON-RPC messages via stdin. Example:

```json
{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2024-11-05","capabilities":{},"clientInfo":{"name":"test-client","version":"1.0.0"}}}
```

```json
{"jsonrpc":"2.0","id":2,"method":"tools/list","params":{}}
```

```json
{"jsonrpc":"2.0","id":3,"method":"tools/call","params":{"name":"financial_projection","arguments":{"projectionInterestRate":0.015,"dateOfRetirement":"2040-12-31","dateOfEndOfSavings":"2040-12-31","retirementAge":{"years":65,"months":0},"finalAge":{"years":70,"months":0},"yearOfBeginProjection":2025,"beginOfRetirementCapital":500000,"retirementCredits":{"40_0":12000,"45_0":14500}}}}
```

## License

This project is part of the PensionTools solution.
