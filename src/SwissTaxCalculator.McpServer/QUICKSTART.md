# Quick Start Guide - Swiss Tax Calculator MCP Server

## Prerequisites

1. **.NET 10 SDK** installed
2. **Visual Studio 2022** or **VS Code** with C# extension

## Step 1: Build the Project

```bash
cd SwissTaxCalculator.McpServer
dotnet build
```

## Step 2: Run the Server

```bash
dotnet run
```

The server will start and listen on standard input/output for MCP protocol messages.

## Step 3: Configure with Claude Desktop

1. Locate your Claude Desktop configuration file:
   - **Windows**: `%APPDATA%\Claude\claude_desktop_config.json`
   - **macOS**: `~/Library/Application Support/Claude/claude_desktop_config.json`
   - **Linux**: `~/.config/Claude/claude_desktop_config.json`

2. Add the Swiss Tax Calculator server:

```json
{
  "mcpServers": {
    "swiss-tax-calculator": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:/workspace/repos/PensionTools/src/SwissTaxCalculator.McpServer/SwissTaxCalculator.McpServer.csproj"
      ]
    }
  }
}
```

*(Adjust the path to match your actual project location)*

3. Restart Claude Desktop

## Step 4: Test the Server

In Claude Desktop, you can now ask questions like:

> "Calculate the wealth and income tax for a married person with 2 children living in Zurich (BFS 261) with a taxable income of 100,000 CHF and wealth of 500,000 CHF for the year 2024"

or

> "What is the capital benefit tax on a 200,000 CHF pension lump sum for a single person in Bern (BFS 351) in 2024?"

## Common Canton BFS Numbers

Here are some commonly used Swiss municipalities:

| City | BFS Number | Canton |
|------|------------|--------|
| Zurich | 261 | ZH |
| Geneva | 6621 | GE |
| Basel | 2701 | BS |
| Bern | 351 | BE |
| Lausanne | 5586 | VD |
| Winterthur | 230 | ZH |
| Lucerne | 1061 | LU |
| St. Gallen | 3203 | SG |
| Lugano | 5192 | TI |
| Biel/Bienne | 371 | BE |

## Example Queries

### Example 1: Wealth and Income Tax

```
Calculate tax for:
- Year: 2024
- Municipality: Zurich (BFS 261), Canton ZH
- Person: Married, 2 children, Protestant
- Taxable income: 150,000 CHF
- Taxable federal income: 150,000 CHF
- Taxable wealth: 1,000,000 CHF
```

### Example 2: Capital Benefit Tax

```
Calculate capital benefit tax for:
- Year: 2024
- Municipality: Geneva (BFS 6621), Canton GE
- Person: Single, Catholic
- Capital benefit: 500,000 CHF (pension lump sum)
```

## Troubleshooting

### Server doesn't start
- Verify .NET 10 SDK is installed: `dotnet --version`
- Check that all dependencies are restored: `dotnet restore`
- Look for build errors: `dotnet build`

### Tools not showing in Claude
- Verify the configuration file path is correct
- Ensure the JSON syntax is valid
- Check that the project path in the config matches your system
- Restart Claude Desktop after configuration changes

### Calculation errors
- Verify the BFS number is correct
- Check that the canton code matches the municipality
- Ensure all required fields are provided
- Check that tax data is available for the requested year

## Next Steps

- Read the full [README.md](README.md) for detailed documentation
- Review the [PROJECT_SUMMARY.md](PROJECT_SUMMARY.md) for architecture details
- Check the [MCP_CONFIGURATION.md](MCP_CONFIGURATION.md) for advanced configuration options
