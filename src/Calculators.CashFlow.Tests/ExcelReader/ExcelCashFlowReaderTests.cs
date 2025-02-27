using System;
using Application.MultiPeriodCalculator.ExcelReader;
using Xunit;
using System.Collections.Generic;
using System.IO;
using Snapshooter.Xunit;

namespace Calculators.CashFlow.Tests.ExcelReader;

public class ExcelCashFlowReaderTests
{
    private const string RelativeTemplateFolderPath = "ExcelReader/Files";

    [Fact]
    public void ReadTaxActions_ValidWorkbook_ReturnsEmptyList()
    {
        // Arrange
        string workbookName = Path.Combine(RelativeTemplateFolderPath, "empty.xlsx");

        // Act
        var result = ExcelCashFlowReader.ReadTaxActions(workbookName);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReadTaxActions_ValidWorkbook_ReturnsExpectedList()
    {
        // Arrange
        string workbookName = Path.Combine(RelativeTemplateFolderPath, "einkauf.xlsx");

        // Act
        var result = ExcelCashFlowReader.ReadTaxActions(workbookName);

        // Assert
        Assert.NotNull(result);
        Snapshot.Match(result);
    }

    [Fact]
    public void ReadMunicipalities_ValidWorkbook_ReturnsEmptyList()
    {
        // Arrange
        string workbookName = Path.Combine(RelativeTemplateFolderPath, "empty.xlsx");

        // Act
        var result = ExcelCashFlowReader.ReadTaxMunicipalities(workbookName);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReadMunicipalities_ValidWorkbook_ReturnsExpectedList()
    {
        // Arrange
        string workbookName = Path.Combine(RelativeTemplateFolderPath, "einkauf.xlsx");

        // Act
        var result = ExcelCashFlowReader.ReadTaxMunicipalities(workbookName);

        // Assert
        Assert.NotNull(result);
        Snapshot.Match(result);
    }

    [Fact]
    public void ReadAccounts_ValidWorkbook_ReturnsEmptyList()
    {
        // Arrange
        string workbookName = Path.Combine(RelativeTemplateFolderPath, "empty.xlsx");

        // Act
        var result = ExcelCashFlowReader.ReadAccounts(workbookName);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReadFixAmountCashFlows_ValidWorkbook_ReturnsEmptyList()
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
    public void ReadAccounts_ValidWorkbook_ReturnsExpectedList()
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
    public void ReadCashFlows_ValidWorkbook_ReturnsExpectedList()
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
    public void ReadPersons_ValidWorkbook_ShouldReturnEmptyList()
    {
        // Arrange
        // Arrange
        string workbookName = Path.Combine(RelativeTemplateFolderPath, "empty.xlsx");

        // Act
        IEnumerable<ExcelPerson> result = ExcelCashFlowReader.ReadPersons(workbookName);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReadPersons_ValidWorkbook_ShouldReturnExpectedList()
    {
        // Arrange
        // Arrange
        string workbookName = Path.Combine(RelativeTemplateFolderPath, "einkauf.xlsx");

        // Act
        IEnumerable<ExcelPerson> result = ExcelCashFlowReader.ReadPersons(workbookName);

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
