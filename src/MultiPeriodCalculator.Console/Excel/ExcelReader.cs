using System.Security.Cryptography;
using System.Text;
using Aspose.Cells;
using Domain.Enums;
using Domain.Models.AccountInputs;

namespace MultiPeriodCalculator.Console.Excel;

public class ExcelReader
{
    public static AccountInput ReadAccountInput(string cashflowDefinitionFullName)
    {
        var accountInput = new AccountInput();

        Workbook workbook = new Workbook(cashflowDefinitionFullName);
        Worksheet worksheet = workbook.Worksheets[3];
        Cells cells = worksheet.Cells;

        List<ExcelAccountDefinition> definitionList = [];

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

        accountInput.IncomeAccounts = definitionList
            .Where(x => x.AccountType == AccountType.Income)
            .Select(x => new IncomeAccountInput { Id = x.AccountId, Description = x.Name, })
            .ToArray();

        accountInput.WealthAccounts = definitionList
            .Where(x => x.AccountType == AccountType.Wealth)
            .Select(x => new WealthAccountInput
            {
                Id = x.AccountId,
                Description = x.Name,
            })
            .ToArray();

        accountInput.ExogenousAccounts = definitionList
            .Where(x => x.AccountType == AccountType.Exogenous)
            .Select(x => new ExogenousAccountInput
            {
                Id = x.AccountId,
                Description = x.Name,
            })
            .ToArray();

        accountInput.ThirdPillarAccounts = definitionList
            .Where(x => x.AccountType == AccountType.ThirdPillar)
            .Select(x => new ThirdPillarAccountInput
            {
                Id = x.AccountId,
                Description = x.Name,
            })
            .ToArray();

        accountInput.OccupationalPensionAccounts = definitionList
            .Where(x => x.AccountType == AccountType.OccupationalPension)
            .Select(x => new OccupationalPensionAccountInput
            {
                Id = x.AccountId,
                Description = x.Name,
            })
            .ToArray();

        return accountInput;
    }

    private static ExcelAccountDefinition? Create(string counter, string? accountName, string? accountTypeName)
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

        return new ExcelAccountDefinition(StringToGuid(counter), accountName ?? "na", accountType);
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
}
