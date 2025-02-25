using System;
using Application.MultiPeriodCalculator.ExcelReader;
using Xunit;
using System.Collections.Generic;
using System.IO;
using Aspose.Cells;
using Snapshooter.Xunit;

namespace Calculators.CashFlow.Tests.ExcelReader;

public class ExcelCashFlowReaderTests
{
    private const string RelativeTemplateFolderPath = "ExcelReader/Files";

    [Fact]
    public void ReadAccountInput_ValidWorkbook_ReturnsEmptyAccountList()
    {
        // Arrange
        string workbookName = Path.Combine(RelativeTemplateFolderPath, "empty.xlsx");

        // Act
        IEnumerable<ExcelAccount> result = ExcelCashFlowReader.ReadAccounts(workbookName);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReadAccountInput_ValidWorkbook_ReturnsEmptyCashFlowList()
    {
        // Arrange
        string workbookName = Path.Combine(RelativeTemplateFolderPath, "empty.xlsx");

        // Act
        IEnumerable<ExcelFixAmountCashFlow> result = ExcelCashFlowReader.ReadCashFlows(workbookName);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReadCashFlowInput_ValidWorkbook_ReturnsExpectedAccountsForPurchase()
    {
        // Arrange
        string workbookName = Path.Combine(RelativeTemplateFolderPath, "einkauf.xlsx");

        // Act
        var result = ExcelCashFlowReader.ReadAccounts(workbookName);

        // Assert
        Assert.NotNull(result);
        Snapshot.Match(result);
    }

    [Fact]
    public void ReadCashFlowInput_ValidWorkbook_ReturnsExpectedCashFlowsForPurchase()
    {
        // Arrange
        string workbookName = Path.Combine(RelativeTemplateFolderPath, "einkauf.xlsx");

        // Act
        IEnumerable<ExcelFixAmountCashFlow> result = ExcelCashFlowReader.ReadCashFlows(workbookName);

        // Assert
        Assert.NotNull(result);
        Snapshot.Match(result);
    }

    [Fact]
    public void StringToGuid_ValidString_ReturnsExpectedGuid()
    {
        // Arrange
        string input = "testString";

        // Act
        Guid result = ExcelCashFlowReader.StringToGuid(input);

        // Assert
        Snapshot.Match(result);
    }

    [Fact]
    public void IntToGuid_ValidInt_ReturnsExpectedGuid()
    {
        // Arrange
        int input = 12345;

        // Act
        Guid result = ExcelCashFlowReader.IntToGuid(input);

        // Assert
        Snapshot.Match(result);
    }
}
