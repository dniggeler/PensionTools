using System;
using System.Collections.Generic;
using System.IO;
using Application.MultiPeriodCalculator.ExcelManager;
using Aspose.Cells;
using Domain.Enums;
using Xunit;

namespace Calculators.CashFlow.Tests.ExcelManager;

public class ExcelCashFlowManagerTests
{
    [Fact]
    public void WriteTransactions_ShouldCreateWorksheetWithTransactions()
    {
        // Arrange
        string filename = Guid.NewGuid().ToString().Substring(0, 6);
        string workbookName = Path.Combine(Path.GetTempPath(), $"{filename}.xlsx");
        Workbook newWorkbook = new Workbook();
        newWorkbook.Save(workbookName);

        Guid accountId = Guid.NewGuid();
        var transactions = new List<ExcelResponseTransaction>
        {
            new ExcelResponseTransaction(accountId, AccountType.Income, "Lohn"),
            new ExcelResponseTransaction(accountId, AccountType.Income, "Lohn")
        };

        // Act
        ExcelCashFlowManager.WriteTransactions(newWorkbook.FileName, transactions);

        // Assert
        Workbook workbook = new Workbook(newWorkbook.FileName);
        Worksheet worksheet = workbook.Worksheets["Transktionen"];
        Assert.NotNull(worksheet);

        File.Delete(workbookName);
    }
}
