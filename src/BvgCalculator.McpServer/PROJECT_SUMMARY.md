# BVG Calculator MCP Server - Project Summary

## Overview

This project implements a Model Context Protocol (MCP) server that exposes the Swiss BVG (Berufliche Vorsorge / Occupational Pension) calculator functionality. It allows AI assistants and other MCP clients to perform pension calculations through standardized protocol tools.

## Project Structure

```
BvgCalculator.McpServer/
??? BvgCalculator.McpServer.csproj    # Project file with dependencies
??? BvgCalculatorMcpServer.cs         # Main MCP server implementation
??? Program.cs                         # Application entry point
??? IntegrationTest.cs                 # Simple integration tests
??? README.md                          # Full documentation
??? QUICKSTART.md                      # Quick start guide
??? MCP_CONFIGURATION.md               # MCP client configuration examples
??? .gitignore                         # Git ignore rules
??? Properties/
    ??? launchSettings.json            # Launch configuration
```

## Key Features

### 5 MCP Tools Exposed

1. **bvg_calculate** - Full pension calculation including:
   - Retirement pension
   - Disability pension
   - Partner pension
   - Orphan pension
   - Child pension for disabled

2. **bvg_insured_salary** - Calculate BVG insured salary for a person/year

3. **bvg_insured_salaries_timeseries** - Project insured salaries until retirement

4. **bvg_retirement_credit_factors** - Age-based credit factor progression

5. **bvg_retirement_credits** - Annual retirement capital contributions

## Technology Stack

- **.NET 10**: Latest .NET framework
- **ModelContextProtocol SDK**: Official MCP .NET implementation
- **LanguageExt**: Functional programming library (Either<L,R> for error handling)
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection
- **Logging**: Microsoft.Extensions.Logging with console provider

## Dependencies

The project references:
- **Application** - Contains IBvgCalculator interface and business logic
- **Domain** - Domain models (BvgPerson, BvgCalculationResult, TechnicalAge, etc.)
- **Infrastructure** - Supporting infrastructure services
- **CommonTypes** - Shared types across the solution
- **CommonUtils** - Utility functions

## Design Principles

1. **Separation of Concerns**: MCP server acts as a thin adapter layer
2. **Error Handling**: Uses Either<string, T> monad for safe error propagation
3. **Dependency Injection**: All services registered and resolved via DI
4. **Structured Logging**: Comprehensive logging for debugging and monitoring
5. **JSON Serialization**: Clean, readable output for AI consumption

## Input/Output Examples

### Input (via MCP client):
```json
{
  "tool": "bvg_calculate",
  "arguments": {
    "calculationYear": 2024,
    "retirementCapitalEndOfYear": 350000,
    "person": {
      "dateOfBirth": "1974-03-15",
      "gender": "Male",
      "reportedSalary": 120000,
      "partTimeDegree": 1.0,
      "disabilityDegree": 0.0
    }
  }
}
```

### Output:
```json
{
  "dateOfRetirement": "2039-01-01",
  "retirementAge": {
    "years": 65,
    "months": 0
  },
  "effectiveSalary": 120000.0,
  "insuredSalary": 62475.0,
  "retirementCredit": 11245.5,
  "retirementPension": 24500.0,
  "disabilityPension": 23800.0,
  "partnerPension": 14280.0,
  "orphanPension": 9520.0,
  "childPensionForDisabled": 9520.0
}
```

## Supported Scenarios

- ? Full-time employees
- ? Part-time workers (adjustable degree)
- ? Disabled persons (partial or full disability)
- ? Male and female (different retirement ages)
- ? Salary above/below BVG thresholds
- ? Time series projections
- ? Historical calculations (any year)

## Compatible MCP Clients

Tested and compatible with:
- **Claude Desktop** (Anthropic)
- **Cline** (VS Code extension)
- **Zed Editor** (with MCP support)
- Any MCP-compatible client using stdio transport

## Development

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run
```

### Test
```bash
dotnet run --project IntegrationTest.cs
```

### Publish
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

## Configuration

The server runs as a stdio (standard input/output) server, which is the MCP standard transport mechanism. No network configuration or ports are required.

Configure in MCP clients by specifying:
- **Command**: `dotnet`
- **Args**: `["run", "--project", "<path-to-project>"]`

Or use the published executable directly.

## Future Enhancements

Potential areas for extension:
- [ ] Add resource endpoints (BVG tables, rate histories)
- [ ] Support prompts for common calculation scenarios
- [ ] Add validation summaries and warnings
- [ ] Expose BVG revision calculations
- [ ] Add comparison tools (e.g., different retirement ages)
- [ ] Caching for frequently accessed data
- [ ] Support for custom BVG plans (beyond minimum)

## Contributing

This server is part of the PensionTools solution. To contribute:
1. Ensure all changes build successfully
2. Test with at least one MCP client
3. Update documentation for new tools
4. Follow existing code style and patterns

## License

Part of the PensionTools project - see main repository for license information.

## Support

For issues or questions:
- Review the documentation files (README.md, QUICKSTART.md, MCP_CONFIGURATION.md)
- Check the MCP specification: https://modelcontextprotocol.io/
- Review the main PensionTools repository

## Version History

- **1.0.0** (Initial Release)
  - 5 core BVG calculation tools
  - Full MCP protocol compliance
  - Support for all person scenarios
  - Comprehensive documentation
