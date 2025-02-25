using Aspose.Cells;
using Domain.Enums;
using AccountInput = Domain.Models.AccountInputs.AccountInput;
using ExogenousAccountInput = Domain.Models.AccountInputs.ExogenousAccountInput;
using IncomeAccountInput = Domain.Models.AccountInputs.IncomeAccountInput;
using OccupationalPensionAccountInput = Domain.Models.AccountInputs.OccupationalPensionAccountInput;
using TaxType = Domain.Enums.TaxType;
using FlowType = Domain.Enums.FlowType;
using ThirdPillarAccountInput = Domain.Models.AccountInputs.ThirdPillarAccountInput;
using WealthAccountInput = Domain.Models.AccountInputs.WealthAccountInput;

namespace Application.MultiPeriodCalculator.ExcelReader;

public class ExcelReader
{
    public static IEnumerable<ExcelAccount> ReadAccountInput(string workbookName)
    {
        Workbook workbook = new Workbook(workbookName);
        Worksheet worksheet = workbook.Worksheets[3];
        Cells cells = worksheet.Cells;

        List<ExcelAccount> definitionList = [];

        int rowCount = cells.MaxDataRow + 1;
        for (int i = 1; i < rowCount; i++)
        {
            string counter = cells[i, 0].StringValue;
            string accountName = cells[i, 1].StringValue;
            string accountType = cells[i, 2].StringValue;

            var excelAccountDefinition = Create(counter, accountName, accountType);
            if (excelAccountDefinition is null)
            {
                continue;
            }

            definitionList.Add(excelAccountDefinition);
        }

        return definitionList;
    }

    public static IEnumerable<ExcelFixAmountCashFlow> ReadCashFlowInput(string workbookName)
    {

        Workbook workbook = new Workbook(workbookName);
        Worksheet worksheet = workbook.Worksheets[4];
        Cells cells = worksheet.Cells;

        List<ExcelFixAmountCashFlow> cashFlowList = [];

        int rowCount = cells.MaxDataRow + 1;
        for (int i = 1; i < rowCount; i++)
        {
            string? processDate = cells[i, 0].StringValue;
            string description = cells[i, 1].StringValue;
            string? debitAccountNumber = cells[i, 2].StringValue;
            string? creditAccountNumber = cells[i, 3].StringValue;
            string amount = cells[i, 4].StringValue;
            string taxType = cells[i, 5].StringValue;
            string flowType = cells[i, 6].StringValue;

            var excelCashFlow = Create(processDate, debitAccountNumber, creditAccountNumber, description, amount, taxType, flowType);
            if (excelCashFlow is null)
            {
                continue;
            }

            cashFlowList.Add(excelCashFlow);
        }

        return cashFlowList;
    }

    private static ExcelAccount? Create(string counter, string? accountName, string? accountTypeName)
    {
        if (string.IsNullOrEmpty(accountTypeName))
        {
            return null;
        }

        AccountType accountType = accountTypeName switch
        {
            "Lohn" => AccountType.Income,
            "Vermögen" => AccountType.Wealth,
            "Exogenous" => AccountType.Exogenous,
            "3a" => AccountType.ThirdPillar,
            "PK" => AccountType.OccupationalPension,

            _ => throw new ArgumentException(nameof(accountTypeName))
        };

        return new ExcelAccount(StringToGuid(counter), accountName ?? "na", accountType);
    }

    private static ExcelFixAmountCashFlow? Create(
        string? processDateString,
        string? debitAccountNumberString,
        string? creditAccountNumberString,
        string? description,
        string amountString,
        string taxTypeString,
        string taxFlowTypeString)
    {
        if (string.IsNullOrEmpty(description) ||
            debitAccountNumberString is null ||
            creditAccountNumberString is null)
        {
            return null;
        }

        if (!DateOnly.TryParse(processDateString, out DateOnly processDate))
        {
            return null;
        }

        if (!int.TryParse(debitAccountNumberString, out int debitAccountNumber))
        {
            return null;
        }

        if (!int.TryParse(creditAccountNumberString, out int creditAccountNumber))
        {
            return null;
        }

        if (!decimal.TryParse(amountString, out decimal amount))
        {
            amount = decimal.Zero;
        }

        TaxType taxType = taxTypeString switch
        {
            "Einkommen" => TaxType.Income,
            "Vermögen" => TaxType.Wealth,
            "Kapitalbezug" => TaxType.CapitalBenefits,
            _ => TaxType.None
        };

        FlowType flowType = taxFlowTypeString switch
        {
            "InFlow" => FlowType.InFlow,
            "OutFlow" => FlowType.OutFlow,
            _ => FlowType.Undefined
        };

        return new ExcelFixAmountCashFlow(
            processDate,
            IntToGuid(debitAccountNumber),
            IntToGuid(creditAccountNumber),
            description,
            amount,
            taxType,
            flowType);
    }

    public static Guid StringToGuid(string input)
    {
        // Convert the input string to a byte array
        byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);

        // Create a byte array to hold the GUID components
        byte[] guidBytes = new byte[16];

        // Copy the input bytes into the GUID byte array
        for (int i = 0; i < Math.Min(inputBytes.Length, guidBytes.Length); i++)
        {
            guidBytes[i] = inputBytes[i];
        }

        // Create the GUID from the byte array
        return new Guid(guidBytes);
    }

    public static Guid IntToGuid(int input)
    {
        return StringToGuid(input.ToString());
    }
}
