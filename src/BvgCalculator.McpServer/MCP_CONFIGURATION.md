# MCP Client Configuration Examples

## Claude Desktop

Add this to your Claude Desktop configuration file:

### Windows
Location: `%APPDATA%\Claude\claude_desktop_config.json`

### macOS
Location: `~/Library/Application Support/Claude/claude_desktop_config.json`

### Configuration
```json
{
  "mcpServers": {
    "bvg-calculator": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:\\path\\to\\PensionTools\\src\\BvgCalculator.McpServer\\BvgCalculator.McpServer.csproj"
      ]
    }
  }
}
```

Or if using a published executable:

```json
{
  "mcpServers": {
    "bvg-calculator": {
      "command": "C:\\path\\to\\BvgCalculator.McpServer.exe",
      "args": []
    }
  }
}
```

## Cline (VS Code Extension)

In your `.cline/cline_mcp_config.json`:

```json
{
  "mcpServers": {
    "bvg-calculator": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "${workspaceFolder}/BvgCalculator.McpServer/BvgCalculator.McpServer.csproj"
      ]
    }
  }
}
```

## Publishing for Standalone Use

To create a standalone executable:

```bash
cd BvgCalculator.McpServer
dotnet publish -c Release -r win-x64 --self-contained
```

Available runtime identifiers:
- `win-x64`: Windows 64-bit
- `linux-x64`: Linux 64-bit
- `osx-x64`: macOS Intel
- `osx-arm64`: macOS Apple Silicon

The executable will be in: `bin/Release/net10.0/{runtime}/publish/`

## Testing the Server

You can test the MCP server using the MCP Inspector:

```bash
npx @modelcontextprotocol/inspector dotnet run --project BvgCalculator.McpServer/BvgCalculator.McpServer.csproj
```

This will open a web interface where you can test the tools interactively.

## Example Prompts for Claude Desktop

Once configured, you can ask Claude:

1. **Calculate BVG Benefits**
   ```
   Calculate the BVG pension benefits for a 50-year-old male born on 1974-03-15, 
   with an annual salary of 120,000 CHF and current retirement capital of 350,000 CHF 
   for the year 2024.
   ```

2. **Get Insured Salary**
   ```
   What is the BVG insured salary for a person born on 1985-07-20, female, 
   with a reported salary of 85,000 CHF in 2024?
   ```

3. **Time Series Analysis**
   ```
   Show me the retirement credit progression for a male born on 1980-01-01 
   with a salary of 100,000 CHF from 2024 until retirement.
   ```

4. **Disability Scenario**
   ```
   Calculate BVG benefits for a person born on 1975-06-10, male, 
   with 80,000 CHF salary, 300,000 CHF retirement capital, 
   and a 50% disability degree in 2024.
   ```
