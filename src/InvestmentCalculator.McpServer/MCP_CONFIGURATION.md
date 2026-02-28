# MCP Configuration Guide for Investment Calculator Server

## Overview

This document provides detailed configuration information for integrating the Investment Calculator MCP Server with various MCP clients.

## Configuration Options

### Basic Configuration

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

### Production Configuration (Published Executable)

After publishing the project with `dotnet publish -c Release`:

```json
{
  "mcpServers": {
    "investment-calculator": {
      "command": "C:/path/to/published/InvestmentCalculator.McpServer.exe"
    }
  }
}
```

### Configuration with Environment Variables

```json
{
  "mcpServers": {
    "investment-calculator": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "${PENSION_TOOLS_ROOT}/InvestmentCalculator.McpServer/InvestmentCalculator.McpServer.csproj"
      ],
      "env": {
        "DOTNET_ENVIRONMENT": "Production",
        "LOG_LEVEL": "Information"
      }
    }
  }
}
```

## Client-Specific Configurations

### Claude Desktop (Windows)

**Config file location**: `%APPDATA%\Claude\claude_desktop_config.json`

```json
{
  "mcpServers": {
    "investment-calculator": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\workspace\\repos\\PensionTools\\src\\InvestmentCalculator.McpServer\\InvestmentCalculator.McpServer.csproj"
      ]
    }
  }
}
```

Note: Use double backslashes (`\\`) in Windows paths within JSON.

### Claude Desktop (macOS/Linux)

**Config file location**: `~/Library/Application Support/Claude/claude_desktop_config.json`

```json
{
  "mcpServers": {
    "investment-calculator": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/Users/username/repos/PensionTools/src/InvestmentCalculator.McpServer/InvestmentCalculator.McpServer.csproj"
      ]
    }
  }
}
```

## Server Capabilities

The Investment Calculator server implements:

- **Protocol Version**: 2024-11-05
- **Transport**: stdio (standard input/output)
- **Tools**: financial_projection

## Tool Schemas

### financial_projection

**Input Schema:**
```json
{
  "type": "object",
  "properties": {
    "projectionInterestRate": {
      "type": "number",
      "description": "Annual interest rate for projection (e.g., 0.02 for 2%)"
    },
    "dateOfRetirement": {
      "type": "string",
      "description": "Date of retirement in ISO format (YYYY-MM-DD)"
    },
    "dateOfEndOfSavings": {
      "type": "string",
      "description": "Date when savings end in ISO format (YYYY-MM-DD)"
    },
    "retirementAge": {
      "type": "object",
      "properties": {
        "years": { "type": "integer" },
        "months": { "type": "integer" }
      },
      "required": ["years", "months"]
    },
    "finalAge": {
      "type": "object",
      "properties": {
        "years": { "type": "integer" },
        "months": { "type": "integer" }
      },
      "required": ["years", "months"]
    },
    "yearOfBeginProjection": {
      "type": "integer",
      "description": "Starting year for the projection"
    },
    "beginOfRetirementCapital": {
      "type": "number",
      "description": "Initial retirement capital at the beginning"
    },
    "retirementCredits": {
      "type": "object",
      "description": "Dictionary of retirement credits by technical age",
      "additionalProperties": { "type": "number" }
    }
  },
  "required": [
    "projectionInterestRate",
    "dateOfRetirement",
    "dateOfEndOfSavings",
    "retirementAge",
    "finalAge",
    "yearOfBeginProjection",
    "beginOfRetirementCapital",
    "retirementCredits"
  ]
}
```

**Output Schema:**
```json
{
  "success": true,
  "data": {
    "projectionTable": [
      {
        "dateOfCalculation": "string (YYYY-MM-DD)",
        "bvgAge": "integer",
        "technicalAge": {
          "years": "integer",
          "months": "integer"
        },
        "proRatedFactor": "number",
        "grossInterestRate": "number",
        "retirementCredit": "number",
        "retirementCapitalWithoutInterest": "number",
        "retirementCapital": "number",
        "isRetirementDate": "boolean",
        "isEndOfSavings": "boolean",
        "isFullYear": "boolean",
        "isFullAge": "boolean"
      }
    ],
    "totalRows": "integer",
    "startDate": "string",
    "endDate": "string",
    "finalCapital": "number"
  }
}
```

## Advanced Configuration

### Multiple Servers

You can configure multiple MCP servers including Investment Calculator:

```json
{
  "mcpServers": {
    "investment-calculator": {
      "command": "dotnet",
      "args": ["run", "--project", "path/to/InvestmentCalculator.McpServer.csproj"]
    },
    "bvg-calculator": {
      "command": "dotnet",
      "args": ["run", "--project", "path/to/BvgCalculator.McpServer.csproj"]
    },
    "tax-calculator": {
      "command": "dotnet",
      "args": ["run", "--project", "path/to/SwissTaxCalculator.McpServer.csproj"]
    }
  }
}
```

### Performance Optimization

For better performance, publish the server in Release mode:

```bash
dotnet publish -c Release -r win-x64 --self-contained false
```

Then reference the published executable:

```json
{
  "mcpServers": {
    "investment-calculator": {
      "command": "C:/published/InvestmentCalculator.McpServer.exe"
    }
  }
}
```

## Debugging

### Enable Detailed Logging

To troubleshoot issues, you can redirect logs to a file (note: this is already configured in the server to avoid interfering with stdio):

The server uses `Microsoft.Extensions.Logging` with `LogLevel.Information` by default.

### Test Connection

You can test the server manually by running it and sending JSON-RPC messages:

```bash
dotnet run
```

Then send:
```json
{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2024-11-05","capabilities":{},"clientInfo":{"name":"test","version":"1.0"}}}
```

Expected response:
```json
{"jsonrpc":"2.0","id":1,"result":{"protocolVersion":"2024-11-05","capabilities":{"tools":{}},"serverInfo":{"name":"investment-calculator-server","version":"1.0.0"}}}
```

## Security Considerations

- The server runs locally and communicates via stdio
- No network ports are opened
- Input validation is performed on all parameters
- Errors are logged but not exposed with sensitive details

## Compatibility

- **MCP Protocol**: 2024-11-05
- **.NET Version**: 10.0
- **Supported Clients**: Any MCP-compatible client (Claude Desktop, etc.)
- **Platforms**: Windows, macOS, Linux (where .NET 10 is supported)

## Troubleshooting

### Server Not Starting
- Verify .NET 10 SDK is installed: `dotnet --version`
- Check project builds successfully: `dotnet build`
- Ensure all dependencies are restored: `dotnet restore`

### Tool Not Appearing in Client
- Restart the MCP client completely
- Verify configuration file syntax (valid JSON)
- Check paths are absolute and correct for your OS
- Look for error messages in client logs

### Calculation Errors
- Ensure dates are in correct ISO format (YYYY-MM-DD)
- Verify ages are logical (retirement age < final age)
- Check retirement credits dictionary format ("years_months": amount)
- Validate interest rate is a decimal (e.g., 0.015 not 1.5 for 1.5%)

## References

- [MCP Protocol Specification](https://modelcontextprotocol.io/)
- [Claude Desktop Configuration](https://docs.anthropic.com/claude/docs)
- [.NET 10 Documentation](https://docs.microsoft.com/dotnet/)
