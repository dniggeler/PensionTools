# MCP Configuration Guide

## Overview

This document provides detailed information on configuring the Swiss Tax Calculator MCP Server with various MCP clients.

## Configuration File Locations

### Claude Desktop

- **Windows**: `%APPDATA%\Claude\claude_desktop_config.json`
- **macOS**: `~/Library/Application Support/Claude/claude_desktop_config.json`
- **Linux**: `~/.config/Claude/claude_desktop_config.json`

### Other MCP Clients

Refer to your specific MCP client's documentation for configuration file location.

## Basic Configuration

### Development Mode (using dotnet run)

```json
{
  "mcpServers": {
    "swiss-tax-calculator": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:/path/to/SwissTaxCalculator.McpServer/SwissTaxCalculator.McpServer.csproj"
      ]
    }
  }
}
```

### Production Mode (using published executable)

First, publish the application:

```bash
cd SwissTaxCalculator.McpServer
dotnet publish -c Release -r win-x64 --self-contained
```

Then configure:

```json
{
  "mcpServers": {
    "swiss-tax-calculator": {
      "command": "C:/path/to/SwissTaxCalculator.McpServer/bin/Release/net10.0/win-x64/publish/SwissTaxCalculator.McpServer.exe"
    }
  }
}
```

## Platform-Specific Configurations

### Windows

```json
{
  "mcpServers": {
    "swiss-tax-calculator": {
      "command": "C:\\workspace\\repos\\PensionTools\\src\\SwissTaxCalculator.McpServer\\bin\\Release\\net10.0\\win-x64\\publish\\SwissTaxCalculator.McpServer.exe"
    }
  }
}
```

### macOS

```json
{
  "mcpServers": {
    "swiss-tax-calculator": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/Users/username/repos/PensionTools/src/SwissTaxCalculator.McpServer/SwissTaxCalculator.McpServer.csproj"
      ]
    }
  }
}
```

Or with published binary:

```json
{
  "mcpServers": {
    "swiss-tax-calculator": {
      "command": "/Users/username/repos/PensionTools/src/SwissTaxCalculator.McpServer/bin/Release/net10.0/osx-x64/publish/SwissTaxCalculator.McpServer"
    }
  }
}
```

### Linux

```json
{
  "mcpServers": {
    "swiss-tax-calculator": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/home/username/repos/PensionTools/src/SwissTaxCalculator.McpServer/SwissTaxCalculator.McpServer.csproj"
      ]
    }
  }
}
```

## Advanced Configuration

### Multiple Servers

You can run multiple MCP servers simultaneously:

```json
{
  "mcpServers": {
    "swiss-tax-calculator": {
      "command": "dotnet",
      "args": ["run", "--project", "path/to/SwissTaxCalculator.McpServer/SwissTaxCalculator.McpServer.csproj"]
    },
    "bvg-calculator": {
      "command": "dotnet",
      "args": ["run", "--project", "path/to/BvgCalculator.McpServer/BvgCalculator.McpServer.csproj"]
    }
  }
}
```

### Environment Variables

You can pass environment variables to configure behavior:

```json
{
  "mcpServers": {
    "swiss-tax-calculator": {
      "command": "dotnet",
      "args": ["run", "--project", "path/to/SwissTaxCalculator.McpServer/SwissTaxCalculator.McpServer.csproj"],
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "LOGGING_LEVEL": "Information"
      }
    }
  }
}
```

## Verification

After configuration, restart your MCP client and verify the server is loaded:

1. Check the MCP client logs for any startup errors
2. Look for "swiss-tax-calculator-server" in the list of available tools
3. Try a simple calculation to verify functionality

## Troubleshooting

### Server Not Loading

1. **Check the path**: Ensure the path to the project or executable is correct
2. **Verify .NET installation**: Run `dotnet --version` to confirm .NET 10 is installed
3. **Check permissions**: Ensure the executable has execute permissions (especially on macOS/Linux)
4. **Review logs**: Check your MCP client's logs for error messages

### Server Crashes on Startup

1. **Check dependencies**: Ensure all NuGet packages are restored
2. **Verify database connections**: If using tax data services, ensure they're accessible
3. **Check configuration**: Ensure appsettings.json (if present) is correctly configured

### Tools Not Appearing

1. **Restart the client**: After configuration changes, always restart the MCP client
2. **Check JSON syntax**: Validate your configuration file JSON syntax
3. **Review protocol version**: Ensure your MCP client supports protocol version "2024-11-05"

## Security Considerations

### Local Development

For local development, running via `dotnet run` is acceptable. The server only listens on stdio and doesn't expose network ports.

### Production Deployment

For production use:

1. **Publish self-contained**: Use `--self-contained` to avoid .NET runtime dependencies
2. **Limit access**: Ensure only authorized users can execute the server
3. **Monitor logs**: Implement logging to track usage and errors
4. **Update regularly**: Keep the server and its dependencies up to date

## Performance Tuning

### Startup Time

To improve startup time:

1. Use published executable instead of `dotnet run`
2. Enable ReadyToRun compilation:

```bash
dotnet publish -c Release -r win-x64 --self-contained /p:PublishReadyToRun=true
```

### Memory Usage

The server is designed to be lightweight. If memory usage is a concern:

1. Monitor with performance tools
2. Consider adjusting logging levels
3. Review cache settings in tax calculator implementations

## References

- [Model Context Protocol Specification](https://modelcontextprotocol.io/)
- [Claude Desktop MCP Documentation](https://docs.anthropic.com/claude/docs)
- [.NET 10 Documentation](https://docs.microsoft.com/dotnet/)
