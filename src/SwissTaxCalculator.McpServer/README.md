# Swiss Tax Calculator MCP Server

A Model Context Protocol (MCP) server implementation that exposes Swiss tax calculation functionality for wealth/income tax and capital benefit tax calculations.

## Overview

This MCP server provides access to Swiss tax calculations through standardized tools that can be consumed by MCP clients. It implements the MCP protocol over stdio (standard input/output), exposing the `IFullWealthAndIncomeTaxCalculator` and `IFullCapitalBenefitTaxCalculator` interfaces as MCP tools.

## Implementation

The server uses a simple stdio-based MCP protocol implementation that:
- Reads JSON-RPC requests from stdin
- Processes tool calls using the Swiss Tax Calculators
- Writes JSON-RPC responses to stdout

This approach ensures maximum compatibility with all MCP clients.

## Available Tools

### 1. `calculate_wealth_and_income_tax`
Calculate Swiss wealth and income tax for a person in a specific municipality.

**Parameters:**
- `calculationYear` (integer): The tax year for calculation
- `municipality` (object): Municipality details
  - `bfsNumber` (integer): Swiss BFS (Federal Statistical Office) number of the municipality
  - `name` (string, optional): Municipality name
  - `canton` (string): Canton code (e.g., 'ZH', 'BE', 'GE')
  - `estvTaxLocationId` (integer, optional): ESTV tax location ID
- `person` (object): Person details
  - `name` (string, optional): Person's name
  - `civilStatus` (string): Civil status - "Single" or "Married"
  - `numberOfChildren` (integer, optional): Number of children (default: 0)
  - `religiousGroupType` (string, optional): Religious group - "Other", "Protestant", "Catholic", "Roman" (default: "Other")
  - `partnerReligiousGroupType` (string, optional): Partner's religious group (for married persons)
  - `taxableIncome` (number): Taxable income amount
  - `taxableFederalIncome` (number): Taxable federal income amount
  - `taxableWealth` (number): Taxable wealth amount
- `withMaxAvailableCalculationYear` (boolean, optional): Use maximum available calculation year if specified year not available (default: false)

**Response:**
```json
{
  "success": true,
  "data": {
    "totalTaxAmount": 12500.50,
    "stateTax": {
      "totalTaxAmount": 8000.25,
      "cantonalTax": 5000.00,
      "municipalTax": 2500.00,
      "churchTax": 500.25,
      "pollTax": 0.00
    },
    "federalTax": {
      "taxAmount": 4500.25,
      "effectiveTaxRate": 0.045,
      "marginalTaxRate": 0.11
    }
  }
}
```

### 2. `calculate_capital_benefit_tax`
Calculate Swiss capital benefit tax (e.g., pension lump sum withdrawal) for a person in a specific municipality.

**Parameters:**
- `calculationYear` (integer): The tax year for calculation
- `municipality` (object): Municipality details (same structure as above)
- `person` (object): Person details
  - `name` (string, optional): Person's name
  - `civilStatus` (string): Civil status - "Single" or "Married"
  - `numberOfChildren` (integer, optional): Number of children (default: 0)
  - `religiousGroupType` (string, optional): Religious group - "Other", "Protestant", "Catholic", "Roman" (default: "Other")
  - `partnerReligiousGroupType` (string, optional): Partner's religious group (for married persons)
  - `taxableCapitalBenefits` (number): Taxable capital benefits amount (e.g., pension lump sum)
- `withMaxAvailableCalculationYear` (boolean, optional): Use maximum available calculation year if specified year not available (default: false)

**Response:**
```json
{
  "success": true,
  "data": {
    "totalTaxAmount": 35000.00,
    "stateResult": {
      "totalTaxAmount": 22000.00,
      "cantonalTax": 15000.00,
      "municipalTax": 6000.00,
      "churchTax": 1000.00
    },
    "federalResult": {
      "taxAmount": 13000.00,
      "effectiveTaxRate": 0.065,
      "marginalTaxRate": 0.11
    }
  }
}
```

## Building and Running

### Prerequisites
- .NET 10 SDK
- Access to the official MCP .NET SDK (ModelContextProtocol NuGet package)
- Required infrastructure services (tax data, municipality data)

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
    "swiss-tax-calculator": {
      "command": "dotnet",
      "args": ["run", "--project", "path/to/SwissTaxCalculator.McpServer/SwissTaxCalculator.McpServer.csproj"]
    }
  }
}
```

Or if using the published executable:

```json
{
  "mcpServers": {
    "swiss-tax-calculator": {
      "command": "path/to/SwissTaxCalculator.McpServer.exe"
    }
  }
}
```

## Example Usage

### Example 1: Calculate Wealth and Income Tax

```json
{
  "calculationYear": 2024,
  "municipality": {
    "bfsNumber": 261,
    "canton": "ZH"
  },
  "person": {
    "civilStatus": "Married",
    "numberOfChildren": 2,
    "religiousGroupType": "Protestant",
    "taxableIncome": 100000,
    "taxableFederalIncome": 100000,
    "taxableWealth": 500000
  }
}
```

### Example 2: Calculate Capital Benefit Tax

```json
{
  "calculationYear": 2024,
  "municipality": {
    "bfsNumber": 351,
    "canton": "BE"
  },
  "person": {
    "civilStatus": "Single",
    "religiousGroupType": "Catholic",
    "taxableCapitalBenefits": 200000
  }
}
```

## Swiss Tax System Notes

### Canton Codes
Swiss cantons use two-letter codes:
- AG (Aargau), AI (Appenzell Innerrhoden), AR (Appenzell Ausserrhoden)
- BE (Bern), BL (Basel-Landschaft), BS (Basel-Stadt)
- FR (Fribourg), GE (Geneva), GL (Glarus), GR (Graubünden)
- JU (Jura), LU (Lucerne), NE (Neuchâtel), NW (Nidwalden)
- OW (Obwalden), SG (St. Gallen), SH (Schaffhausen), SO (Solothurn)
- SZ (Schwyz), TG (Thurgau), TI (Ticino), UR (Uri)
- VD (Vaud), VS (Valais), ZG (Zug), ZH (Zurich)

### Civil Status
- **Single**: Includes single, widowed, and divorced persons
- **Married**: Married persons filing jointly

### Religious Groups
- **Other**: No church affiliation or undefined
- **Protestant**: Reformed church
- **Catholic**: Catholic church
- **Roman**: Roman Catholic church

Church tax is only applicable in certain cantons and for members of recognized religious groups.

## Error Handling

The server returns structured error responses:

```json
{
  "success": false,
  "error": "Error message describing what went wrong"
}
```

Common errors include:
- Invalid canton code
- Municipality not found
- Invalid civil status
- Missing required fields
- Calculation year not available in tax data

## Dependencies

This MCP server depends on:
- Application layer (tax calculation contracts and implementations)
- Domain models (tax persons, municipalities, results)
- Infrastructure layer (tax data, municipality repository)

## Architecture

The server follows a simple architecture:
1. **Program.cs**: Entry point, handles MCP protocol communication
2. **SwissTaxCalculatorService.cs**: Service layer that wraps the tax calculators
3. JSON serialization/deserialization for tool inputs and outputs
4. Error handling and logging

## Limitations

- Tax data availability depends on the configured tax calculator implementation (ESTV API, proprietary, or mock)
- Some municipalities may not have tax data for all years
- The `withMaxAvailableCalculationYear` flag can be used to fall back to the latest available year
