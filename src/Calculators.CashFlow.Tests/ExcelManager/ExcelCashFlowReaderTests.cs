using System;
using System.Collections.Generic;
using System.IO;
using Application.MultiPeriodCalculator.ExcelManager;
using Snapshooter.Xunit;
using Xunit;

namespace Calculators.CashFlow.Tests.ExcelManager;

public class ExcelCashFlowReaderTests
{
    private const string RelativeTemplateFolderPath = "ExcelManager/Files";

    [Fact]
    public void ReadTaxActions_ValidWorkbook_ReturnsEmptyList()
    {
        // Arrange
        string workbookName = Path.Combine(RelativeTemplateFolderPath, "empty.xlsx");

        // Act
        var result = ExcelCashFlowManager.ReadTaxActions(workbookName);

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
        var result = ExcelCashFlowManager.ReadTaxActions(workbookName);

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
        var result = ExcelCashFlowManager.ReadTaxMunicipalities(workbookName);

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
        var result = ExcelCashFlowManager.ReadTaxMunicipalities(workbookName);

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
        var result = ExcelCashFlowManager.ReadAccounts(workbookName);

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
        IEnumerable<ExcelFixAmountCashFlow> result = ExcelCashFlowManager.ReadFixAmountCashFlows(workbookName);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ReadTransferRatioCashFlows_ValidWorkbook_ReturnsEmptyList()
    {
        // Arrange
        string workbookName = Path.Combine(RelativeTemplateFolderPath, "empty.xlsx");

        // Act
        IEnumerable<ExcelTransferRatioCashFlow> result = ExcelCashFlowManager.ReadTransferRatioCashFlows(workbookName);

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
        var result = ExcelCashFlowManager.ReadAccounts(workbookName);

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
        IEnumerable<ExcelFixAmountCashFlow> result = ExcelCashFlowManager.ReadFixAmountCashFlows(workbookName);

        // Assert
        Assert.NotNull(result);
        Snapshot.Match(result);
    }

    [Fact]
    public void ReadTransferRatioCashFlows_ValidWorkbook_ReturnsExpectedList()
    {
        // Arrange
        string workbookName = Path.Combine(RelativeTemplateFolderPath, "einkauf.xlsx");

        // Act
        IEnumerable<ExcelTransferRatioCashFlow> result = ExcelCashFlowManager.ReadTransferRatioCashFlows(workbookName);

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
        IEnumerable<ExcelPerson> result = ExcelCashFlowManager.ReadPersons(workbookName);

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
        IEnumerable<ExcelPerson> result = ExcelCashFlowManager.ReadPersons(workbookName);

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
        Guid result = ExcelCashFlowManager.StringToGuid(input);

        // Assert
        Snapshot.Match(result);
    }

    [Fact]
    public void IntToGuid_ValidInt_ReturnsExpectedGuid()
    {
        // Arrange
        int input = 12345;

        // Act
        Guid result = ExcelCashFlowManager.IntToGuid(input);

        // Assert
        Snapshot.Match(result);
    }
}
