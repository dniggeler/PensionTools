# BVG Calculator MCP Server

A Model Context Protocol (MCP) server implementation that exposes the Swiss BVG (occupational pension) calculator functionality.

## Overview

This MCP server provides access to BVG calculations through standardized tools that can be consumed by MCP clients. It implements the MCP protocol over stdio (standard input/output), exposing the `IBvgCalculator` interface methods as MCP tools.

## Implementation

The server uses a simple stdio-based MCP protocol implementation that:
- Reads JSON-RPC requests from stdin
- Processes tool calls using the BVG Calculator
- Writes JSON-RPC responses to stdout

This approach ensures maximum compatibility with all MCP clients.

## Available Tools

### 1. `bvg_calculate`
Calculate comprehensive BVG benefits including:
- Retirement pension
- Disability pension
- Partner pension
- Orphan pension
- Child pension for disabled

**Parameters:**
- `calculationYear` (integer): The year for which to calculate benefits
- `retirementCapitalEndOfYear` (number): The retirement capital at end of year
- `person` (object): Person details
  - `dateOfBirth` (string): Date of birth in ISO format (YYYY-MM-DD)
  - `gender` (string): "Male" or "Female"
  - `reportedSalary` (number): Annual salary
  - `partTimeDegree` (number, optional): Part-time degree (0.0-1.0, default 1.0)
  - `disabilityDegree` (number, optional): Disability degree (0.0-1.0, default 0.0)

### 2. `bvg_insured_salary`
Calculate the BVG insured salary for a given year and person.

**Parameters:**
- `calculationYear` (integer): The year for the calculation
- `person` (object): Person details (same structure as above)

### 3. `bvg_insured_salaries_timeseries`
Get a time series of insured salaries from calculation year until retirement.

**Parameters:**
- `calculationYear` (integer): Starting year
- `person` (object): Person details (same structure as above)

### 4. `bvg_retirement_credit_factors`
Get a time series of retirement credit factors by age.

**Parameters:**
- `calculationYear` (integer): The calculation year
- `person` (object): Person details (same structure as above)

### 5. `bvg_retirement_credits`
Get a time series of annual retirement credits (contributions to retirement capital).

**Parameters:**
- `calculationYear` (integer): The calculation year
- `person` (object): Person details (same structure as above)

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
    "bvg-calculator": {
      "command": "dotnet",
      "args": ["run", "--project", "path/to/BvgCalculator.McpServer/BvgCalculator.McpServer.csproj"]
    }
  }
}
```

Or if using the published executable:

```json
{
  "mcpServers": {
    "bvg-calculator": {
      "command": "path/to/BvgCalculator.McpServer.exe"
    }
  }
}
```

## Example Usage

Once connected to an MCP client, you can use the tools like this:

```
Calculate BVG benefits for a 45-year-old male with a salary of 100,000 CHF 
and retirement capital of 250,000 CHF in 2024.
```

The MCP client will invoke:
```json
{
  "tool": "bvg_calculate",
  "arguments": {
    "calculationYear": 2024,
    "retirementCapitalEndOfYear": 250000,
    "person": {
      "dateOfBirth": "1979-01-15",
      "gender": "Male",
      "reportedSalary": 100000,
      "partTimeDegree": 1.0,
      "disabilityDegree": 0.0
    }
  }
}
```

## Dependencies

This project references:
- **Application**: Contains the IBvgCalculator interface and implementation
- **Domain**: Contains domain models (BvgPerson, BvgCalculationResult, etc.)
- **Infrastructure**: Contains supporting infrastructure services
- **ModelContextProtocol**: Official MCP .NET SDK

## Architecture

The server follows these design principles:
- Uses dependency injection for service management
- Leverages the official MCP .NET SDK
- Handles errors gracefully with Either<string, T> pattern
- Returns results in JSON format for easy consumption
- Provides detailed parameter schemas for tool discovery

## Error Handling

All tools return either:
- **Success**: JSON-formatted result with the requested data
- **Error**: Error message with `isError: true` flag

Validation errors from the underlying BVG calculator are also captured and returned in a user-friendly format.
