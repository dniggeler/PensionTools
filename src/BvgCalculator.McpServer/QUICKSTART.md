# BVG Calculator MCP Server - Quick Start Guide

## What is this?

The BVG Calculator MCP Server exposes Swiss occupational pension (BVG) calculation functionality through the Model Context Protocol (MCP). This allows AI assistants like Claude to perform BVG calculations directly.

## Quick Setup (5 minutes)

### Step 1: Build the Project

```bash
cd BvgCalculator.McpServer
dotnet build
```

### Step 2: Test the Server

Run the server to verify it works:

```bash
dotnet run
```

The server will start and wait for MCP client connections via stdio.

### Step 3: Configure your MCP client (example for GitHub Copilot)

Add the BVG MCP server to your MCP client configuration so Copilot can call the tools. Below is an example JSON entry you can adapt for your environment. The exact location of the MCP configuration depends on the Copilot client you use; consult its documentation for where to place MCP server entries.

Example configuration:

```json
{
  "mcpServers": {
    "bvg-calculator": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:/path/to/PensionTools/src/BvgCalculator.McpServer/BvgCalculator.McpServer.csproj"
      ]
    }
  }
}
```

After updating your MCP configuration, restart GitHub Copilot (or the Copilot client) so it discovers the new server.

### Step 4: Test It!

Open Claude Desktop and try:

```
Calculate BVG benefits for a 45-year-old male with 100,000 CHF salary 
and 250,000 CHF retirement capital in 2024.
```

Claude will use the MCP server to perform the calculation!

## What Can It Do?

The server provides 5 BVG calculation tools:

1. **Full BVG Calculation** - Retirement, disability, partner, and orphan pensions
2. **Insured Salary** - Calculate BVG insured salary
3. **Salary Time Series** - Projected insured salaries until retirement
4. **Credit Factors** - Age-based retirement credit factors
5. **Retirement Credits** - Annual contributions to retirement capital

## Common Use Cases

### Example 1: Standard Calculation
```
What will be the retirement pension for someone born in 1970, male, 
with 120,000 CHF annual salary and 400,000 CHF in retirement capital?
```

### Example 2: Disability Scenario
```
Calculate BVG benefits for a 40% disabled person born in 1980, 
currently earning 60,000 CHF with 200,000 CHF retirement capital.
```

### Example 3: Part-Time Worker
```
Calculate insured salary for a part-time worker (60%) born in 1990, 
earning 50,000 CHF in 2024.
```

### Example 4: Timeline Analysis
```
Show me the progression of retirement credits from age 25 to retirement 
for someone born in 1995 with 90,000 CHF salary.
```

## Troubleshooting

### Server won't start
- Verify .NET 10 SDK is installed: `dotnet --version`
- Check all dependencies build: `dotnet build`
- Review logs in the console output

### Claude doesn't see the tools
- Verify the path in `claude_desktop_config.json` is correct
- Restart Claude Desktop after config changes
- Check Claude's developer tools (View > Toggle Developer Tools) for errors

### Calculations return errors
- Verify input parameters (dates in YYYY-MM-DD format)
- Check that salaries and amounts are reasonable values
- Review the error message returned by the tool

## Advanced: Publishing

Create a standalone executable:

```bash
# Windows
dotnet publish -c Release -r win-x64 --self-contained

# macOS (Apple Silicon)
dotnet publish -c Release -r osx-arm64 --self-contained

# Linux
dotnet publish -c Release -r linux-x64 --self-contained
```

Then update your MCP config to point to the executable instead of using `dotnet run`.

## Need Help?

- Check the full [README.md](README.md) for detailed documentation
- Review [MCP_CONFIGURATION.md](MCP_CONFIGURATION.md) for more config options
- See the [Model Context Protocol documentation](https://modelcontextprotocol.io/)

## Next Steps

- Integrate with other MCP clients (VS Code, Zed, etc.)
- Extend with additional BVG calculation features
- Create custom tools for specific use cases
