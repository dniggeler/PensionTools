# Investment Calculator MCP Server - Quick Start Guide

## What is this?

The Investment Calculator MCP Server is a tool that allows AI assistants (like Claude) to perform detailed financial projections for retirement savings and investments. It calculates month-by-month projections of retirement capital with interest compounding and retirement credits.

## Quick Setup (5 minutes)

### Step 1: Build the Server

```bash
cd InvestmentCalculator.McpServer
dotnet build
```

### Step 2: Configure Claude Desktop

1. Open your Claude Desktop configuration file:
   - **Windows**: `%APPDATA%\Claude\claude_desktop_config.json`
   - **macOS**: `~/Library/Application Support/Claude/claude_desktop_config.json`

2. Add this configuration (adjust the path to match your setup):

```json
{
  "mcpServers": {
    "investment-calculator": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:/workspace/repos/PensionTools/src/InvestmentCalculator.McpServer/InvestmentCalculator.McpServer.csproj"
      ]
    }
  }
}
```

### Step 3: Restart Claude Desktop

Close and reopen Claude Desktop completely.

### Step 4: Verify It's Working

In Claude Desktop, you should see a small hammer icon (??) indicating available tools. Try asking:

> "Can you calculate a financial projection for retirement savings starting with 500,000 CHF, with 1.5% annual interest, retiring on 2040-12-31, projecting from 2025 to age 70?"

## Example Usage

### Basic Projection

Ask Claude:
> "Calculate a retirement savings projection with:
> - Starting capital: 500,000 CHF
> - Interest rate: 1.5% annually
> - Retirement date: December 31, 2040 (age 65)
> - Project until age 70
> - Start projection in 2025
> - Annual retirement credits of 15,000 CHF from age 40-65"

Claude will use the `financial_projection` tool to calculate a detailed month-by-month projection.

### Advanced Scenarios

You can also ask for:
- Comparisons of different interest rates
- Impact of different retirement ages
- Effects of varying contribution amounts
- Analysis of capital growth over time

## What Data Does It Calculate?

For each month in the projection period:
- Retirement capital (with and without interest)
- Accumulated retirement credits
- Interest earned
- Pro-rated factors for partial years
- Markers for important dates (retirement, end of savings)

## Troubleshooting

### Tool not appearing in Claude?
- Verify the path in the config file is correct
- Make sure Claude Desktop was restarted after config changes
- Check that the project builds successfully

### Errors when running calculations?
- Ensure dates are in ISO format (YYYY-MM-DD)
- Verify retirement age is less than final age
- Check that retirement credits dictionary uses proper format ("years_months": amount)

### Build errors?
- Confirm .NET 10 SDK is installed
- Run `dotnet restore` before building
- Check that all project references resolve correctly

## Next Steps

- Read the full [README.md](README.md) for detailed parameter descriptions
- Check [MCP_CONFIGURATION.md](MCP_CONFIGURATION.md) for advanced configuration options
- See [PROJECT_SUMMARY.md](PROJECT_SUMMARY.md) for technical architecture details

## Support

For issues or questions:
- Check the existing MCP servers (BvgCalculator, SwissTaxCalculator) for similar patterns
- Review the Application layer for calculator implementation details
- Consult the Domain layer for model definitions
