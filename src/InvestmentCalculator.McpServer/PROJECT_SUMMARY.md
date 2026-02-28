# Investment Calculator MCP Server - Project Summary

## Project Overview

The Investment Calculator MCP Server is a .NET 10 application that exposes financial projection calculation capabilities through the Model Context Protocol (MCP). It allows AI assistants and other MCP clients to perform detailed retirement savings projections with compound interest and periodic contributions.

## Architecture

### Layer Structure

```
InvestmentCalculator.McpServer/
??? Program.cs                      # MCP protocol handler (stdio JSON-RPC)
??? InvestmentCalculatorService.cs  # Service layer wrapper
??? Dependencies:
    ??? Application/
    ?   ??? Bvg/
    ?       ??? ISavingsProcessProjectionCalculator.cs  # Core interface
    ?       ??? SingleSavingsProcessProjectionCalculator.cs  # Implementation
    ??? Domain/
    ?   ??? Models/Bvg/
    ?       ??? TechnicalAge.cs
    ?       ??? RetirementSavingsProcessResult.cs
    ??? Infrastructure/
```

### Component Responsibilities

#### Program.cs
- Implements MCP protocol over stdio
- Handles JSON-RPC message parsing and routing
- Manages server lifecycle and cancellation
- Responds to protocol methods: `initialize`, `tools/list`, `tools/call`

#### InvestmentCalculatorService.cs
- Wraps `ISavingsProcessProjectionCalculator` for MCP exposure
- Handles JSON parameter parsing and validation
- Transforms domain models to JSON responses
- Provides error handling and logging

#### ISavingsProcessProjectionCalculator
- Core business logic interface
- Calculates month-by-month financial projections
- Handles interest compounding and contribution schedules

## MCP Protocol Implementation

### Supported Methods

1. **initialize**
   - Establishes protocol version (2024-11-05)
   - Returns server capabilities
   - Provides server identification

2. **tools/list**
   - Returns available tools (financial_projection)
   - Includes complete JSON schema for tool parameters
   - Describes input and output formats

3. **tools/call**
   - Executes financial_projection tool
   - Returns calculation results in structured format
   - Handles errors gracefully

### Transport Layer

- **Protocol**: JSON-RPC 2.0 over stdio
- **Input**: stdin (standard input)
- **Output**: stdout (standard output)
- **Logging**: Configured to avoid stdio interference

## Data Models

### Input: Financial Projection Parameters

```csharp
{
    projectionInterestRate: decimal,      // e.g., 0.015 for 1.5%
    dateOfRetirement: DateTime,           // ISO 8601 format
    dateOfEndOfSavings: DateTime,         // ISO 8601 format
    retirementAge: TechnicalAge,          // {years, months}
    finalAge: TechnicalAge,               // {years, months}
    yearOfBeginProjection: int,           // e.g., 2025
    beginOfRetirementCapital: decimal,    // Initial capital
    retirementCredits: Dictionary<string, decimal>  // "years_months": amount
}
```

### Output: Projection Results

```csharp
{
    success: bool,
    data: {
        projectionTable: RetirementSavingsProcessResult[],
        totalRows: int,
        startDate: string,
        endDate: string,
        finalCapital: decimal
    }
}
```

### RetirementSavingsProcessResult

```csharp
record RetirementSavingsProcessResult
{
    DateTime DateOfCalculation;
    int BvgAge;
    TechnicalAge TechnicalAge;
    decimal ProRatedFactor;          // For partial years
    decimal GrossInterestRate;
    decimal RetirementCredit;
    decimal RetirementCapitalWithoutInterest;
    decimal RetirementCapital;       // With compound interest
    bool IsRetirementDate;
    bool IsEndOfSavings;
    bool IsFullYear;
    bool IsFullAge;
}
```

## Calculation Algorithm

The projection calculator performs the following:

1. **Initialization**
   - Calculate technical birth date from retirement date and age
   - Determine projection period (start to final age)
   - Initialize capital values

2. **Monthly Iteration**
   - For each month from start to final age:
     - Calculate pro-rated factor (? = months/12)
     - Get retirement credit for current technical age
     - Calculate capital without interest: AGH_oz + (retirement_credit × ?)
     - Calculate capital with interest: AGH_mz × (1 + ? × rate) + (retirement_credit × ?)
     - Track flags (retirement date, end of savings, etc.)

3. **Year-end Processing**
   - Update base capital values
   - Apply full year's retirement credit
   - Compound full year's interest
   - Reset pro-rated counter

## Dependencies

### NuGet Packages
- **ModelContextProtocol** (0.7.0-preview.1): MCP protocol support
- **Microsoft.Extensions.DependencyInjection** (10.0.0): DI container
- **Microsoft.Extensions.Logging** (10.0.0): Logging infrastructure

### Project References
- **Application.csproj**: Business logic and calculators
- **Domain.csproj**: Domain models and interfaces
- **Infrastructure.csproj**: Infrastructure concerns

## Service Registration

```csharp
services.AddBvgCalculators();  // From Application.Extensions
services.AddSingleton<InvestmentCalculatorService>();
```

The `AddBvgCalculators()` extension registers:
- `ISavingsProcessProjectionCalculator` ? `SingleSavingsProcessProjectionCalculator`
- `IBvgRetirementCredits` ? `BvgRetirementCreditsTable`
- `IBvgCalculator` ? `BvgCalculator`
- Related validators

## Error Handling

### Levels of Error Handling

1. **Protocol Level** (Program.cs)
   - JSON parsing errors
   - Unknown method errors
   - Malformed request errors

2. **Service Level** (InvestmentCalculatorService.cs)
   - Parameter validation errors
   - Missing required fields
   - Type conversion errors

3. **Business Logic Level** (Calculator implementations)
   - Domain validation errors
   - Calculation constraint violations

### Error Response Format

```json
{
  "success": false,
  "error": "Error message",
  "stackTrace": "Detailed stack trace (in debug mode)"
}
```

## Logging Strategy

- **Level**: Information (default)
- **Target**: Internal logging system (not stdout/stderr)
- **Purpose**: Debug and troubleshooting without interfering with MCP protocol

## Testing Strategy

### Manual Testing
1. Run server: `dotnet run`
2. Send JSON-RPC initialize message
3. List tools
4. Call financial_projection with test data
5. Verify response format and calculations

### Integration Testing
- Test through MCP client (Claude Desktop)
- Verify tool appears in client
- Execute various calculation scenarios
- Validate results

### Unit Testing Opportunities
- InvestmentCalculatorService JSON parsing
- Parameter validation logic
- Edge cases (empty periods, zero interest, etc.)

## Performance Considerations

- **Memory**: Projection table size grows with date range
- **CPU**: Monthly calculations are O(n) where n = months in range
- **Startup**: Fast (minimal dependencies)
- **Optimization**: Consider caching for repeated calculations with same parameters

## Security Considerations

- **No Network Exposure**: stdio only
- **Input Validation**: All parameters validated before processing
- **Error Messages**: Generic messages to avoid information leakage
- **Logging**: Sensitive data not logged

## Future Enhancements

1. **Additional Tools**
   - Investment comparison tool
   - Risk analysis calculator
   - Tax-adjusted projections
   - Monte Carlo simulations

2. **Performance Improvements**
   - Parallel calculations for multiple scenarios
   - Result caching mechanism
   - Batch projection calculations

3. **Features**
   - Support for multiple currency scenarios
   - Inflation adjustment calculations
   - Variable interest rate schedules
   - Custom contribution patterns

4. **Integration**
   - Direct integration with tax calculator
   - BVG calculator synergy
   - Export to Excel/PDF

## Code Quality Standards

- **.NET 10** features and idioms
- **Nullable reference types** enabled
- **Record types** for immutable data
- **Dependency Injection** throughout
- **Logging** via Microsoft.Extensions.Logging
- **Error handling** with explicit try-catch blocks

## Deployment

### Development
```bash
dotnet run
```

### Production
```bash
dotnet publish -c Release -r win-x64 --self-contained false
```

### Configuration
- See `MCP_CONFIGURATION.md` for client setup
- See `QUICKSTART.md` for getting started

## Related Projects

- **BvgCalculator.McpServer**: Swiss occupational pension calculations
- **SwissTaxCalculator.McpServer**: Swiss tax calculations
- Both share similar MCP server patterns and architecture

## Maintainability

- **Clear separation** between protocol handling and business logic
- **Testable** service layer
- **Documented** through inline comments and markdown files
- **Consistent** with other MCP servers in solution
- **Extensible** through clean interfaces

## References

- Model Context Protocol: https://modelcontextprotocol.io/
- MCP .NET SDK: https://github.com/modelcontextprotocol/dotnet-sdk
- Swiss BVG System: https://www.bsv.admin.ch/
- PensionTools Repository: https://github.com/dniggeler/PensionTools
