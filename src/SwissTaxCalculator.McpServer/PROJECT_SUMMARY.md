# Project Summary - Swiss Tax Calculator MCP Server

## Overview

The Swiss Tax Calculator MCP Server is a Model Context Protocol (MCP) implementation that exposes Swiss tax calculation functionality through standardized tools. It provides two main calculation services:

1. **Wealth and Income Tax Calculator** - Calculates combined cantonal, municipal, church, and federal taxes based on income and wealth
2. **Capital Benefit Tax Calculator** - Calculates taxes on capital benefits such as pension lump sum withdrawals

## Architecture

### Components

```
SwissTaxCalculator.McpServer
??? Program.cs                          # MCP protocol handler and entry point
??? SwissTaxCalculatorService.cs        # Service layer wrapping tax calculators
??? SwissTaxCalculator.McpServer.csproj # Project file
??? README.md                           # Main documentation
??? QUICKSTART.md                       # Quick start guide
??? MCP_CONFIGURATION.md                # Configuration guide
??? PROJECT_SUMMARY.md                  # This file
```

### Dependencies

The server depends on the following project components:

1. **Application Layer**
   - `Application.Tax.Contracts.IFullWealthAndIncomeTaxCalculator`
   - `Application.Tax.Contracts.IFullCapitalBenefitTaxCalculator`
   - `Application.Features.FullTaxCalculation` (service registration)
   
2. **Domain Layer**
   - `Domain.Models.Tax.TaxPerson`
   - `Domain.Models.Tax.CapitalBenefitTaxPerson`
   - `Domain.Models.Tax.FullTaxResult`
   - `Domain.Models.Tax.FullCapitalBenefitTaxResult`
   - `Domain.Models.Municipality.MunicipalityModel`
   - `Domain.Enums` (Canton, CivilStatus, ReligiousGroupType)

3. **Infrastructure Layer**
   - `Infrastructure.DataStaging` (tax data services)
   - `Infrastructure.Tax.Data` (tax data repository)

### Design Patterns

1. **Adapter Pattern**: The server acts as an adapter between the MCP protocol and the tax calculator interfaces
2. **Dependency Injection**: Uses Microsoft.Extensions.DependencyInjection for service resolution
3. **Functional Programming**: Leverages LanguageExt's `Either<L, R>` for error handling
4. **JSON-RPC**: Implements JSON-RPC 2.0 protocol for MCP communication

## Protocol Implementation

### MCP Methods Supported

1. **initialize**: Handshake and capability negotiation
2. **tools/list**: Returns available tools and their schemas
3. **tools/call**: Executes a tool with provided arguments

### Tools Exposed

#### 1. calculate_wealth_and_income_tax

**Purpose**: Calculate comprehensive Swiss taxes including cantonal, municipal, church, and federal taxes based on income and wealth.

**Input Schema**:
```typescript
{
  calculationYear: number;
  municipality: {
    bfsNumber: number;
    canton: string;
    name?: string;
    estvTaxLocationId?: number;
  };
  person: {
    civilStatus: "Single" | "Married";
    numberOfChildren?: number;
    religiousGroupType?: "Other" | "Protestant" | "Catholic" | "Roman";
    partnerReligiousGroupType?: string;
    taxableIncome: number;
    taxableFederalIncome: number;
    taxableWealth: number;
    name?: string;
  };
  withMaxAvailableCalculationYear?: boolean;
}
```

**Output**:
```typescript
{
  success: boolean;
  data?: {
    totalTaxAmount: number;
    stateTax: {
      totalTaxAmount: number;
      cantonalTax: number;
      municipalTax: number;
      churchTax: number;
      pollTax: number;
    };
    federalTax: {
      taxAmount: number;
      effectiveTaxRate: number;
      marginalTaxRate: number;
    };
  };
  error?: string;
}
```

#### 2. calculate_capital_benefit_tax

**Purpose**: Calculate taxes on capital benefits (e.g., pension lump sum withdrawals).

**Input Schema**:
```typescript
{
  calculationYear: number;
  municipality: {
    bfsNumber: number;
    canton: string;
    name?: string;
    estvTaxLocationId?: number;
  };
  person: {
    civilStatus: "Single" | "Married";
    numberOfChildren?: number;
    religiousGroupType?: "Other" | "Protestant" | "Catholic" | "Roman";
    partnerReligiousGroupType?: string;
    taxableCapitalBenefits: number;
    name?: string;
  };
  withMaxAvailableCalculationYear?: boolean;
}
```

**Output**:
```typescript
{
  success: boolean;
  data?: {
    totalTaxAmount: number;
    stateResult: {
      totalTaxAmount: number;
      cantonalTax: number;
      municipalTax: number;
      churchTax: number;
    };
    federalResult: {
      taxAmount: number;
      effectiveTaxRate: number;
      marginalTaxRate: number;
    };
  };
  error?: string;
}
```

## Service Implementation

### SwissTaxCalculatorService

The service layer handles:

1. **JSON Parsing**: Converts MCP tool arguments to domain models
2. **Service Invocation**: Calls the appropriate tax calculator interface
3. **Result Mapping**: Transforms domain results to JSON responses
4. **Error Handling**: Catches exceptions and returns structured error responses

### Key Methods

- `CalculateWealthAndIncomeTax`: Wrapper for `IFullWealthAndIncomeTaxCalculator.CalculateAsync`
- `CalculateCapitalBenefitTax`: Wrapper for `IFullCapitalBenefitTaxCalculator.CalculateAsync`
- `ParseMunicipality`: Parses JSON municipality data to `MunicipalityModel`
- `ParseTaxPerson`: Parses JSON person data to `TaxPerson`
- `ParseCapitalBenefitTaxPerson`: Parses JSON person data to `CapitalBenefitTaxPerson`

## Error Handling

The server implements comprehensive error handling:

1. **Protocol Errors**: Invalid JSON-RPC requests return standard error responses
2. **Calculation Errors**: Tax calculator errors are wrapped in the response with `success: false`
3. **Exception Handling**: Unhandled exceptions are caught and logged
4. **Graceful Shutdown**: Handles Ctrl+C for clean shutdown

### Error Response Format

```json
{
  "success": false,
  "error": "Error message describing the issue"
}
```

or for protocol errors:

```json
{
  "jsonrpc": "2.0",
  "error": {
    "code": -32603,
    "message": "Error message"
  }
}
```

## Logging

The server uses Microsoft.Extensions.Logging for structured logging:

- **Information**: Normal operation events
- **Warning**: Non-critical issues
- **Error**: Exception details with stack traces

Logs are written to the console but can be redirected in production.

## Testing

### Manual Testing

1. Use the MCP protocol inspector or Claude Desktop
2. Invoke tools with sample data
3. Verify responses match expected format

### Integration Testing

Future enhancement: Add integration tests using the MCP test harness.

## Future Enhancements

1. **Additional Tools**
   - Marginal tax rate calculator
   - Tax comparison across municipalities
   - Multi-year tax projections

2. **Performance**
   - Caching frequently accessed tax data
   - Batch calculation support
   - Async streaming for large result sets

3. **Monitoring**
   - Structured logging to file
   - Performance metrics
   - Usage analytics

4. **Security**
   - Input validation and sanitization
   - Rate limiting
   - Authentication (if needed)

## Swiss Tax System Context

### Tax Components

1. **Cantonal Tax**: Tax levied by the canton
2. **Municipal Tax**: Tax levied by the municipality (usually a multiplier of cantonal tax)
3. **Church Tax**: Optional tax for members of recognized churches
4. **Federal Tax**: Direct federal tax (separate calculation)
5. **Poll Tax**: Fixed minimum tax in some cantons

### Special Considerations

- **Civil Status**: Married couples are taxed jointly with different rates
- **Children**: Number of children affects deductions and rates
- **Religious Affiliation**: Determines church tax liability
- **Capital Benefits**: Taxed separately at privileged rates (usually 1/5 of normal rate)

### BFS Numbers

The Swiss Federal Statistical Office (BFS) assigns unique numbers to all municipalities. These are used to identify the tax jurisdiction.

## References

- Model Context Protocol: https://modelcontextprotocol.io/
- Swiss Tax Administration (ESTV): https://www.estv.admin.ch/
- Swiss Federal Statistical Office: https://www.bfs.admin.ch/
