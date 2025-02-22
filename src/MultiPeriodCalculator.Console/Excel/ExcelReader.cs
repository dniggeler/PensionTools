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
            .Select(x => new IncomeAccountInput { Id = Guid.NewGuid(), Description = x.Name, })
            .ToArray();

        accountInput.WealthAccounts = definitionList
            .Where(x => x.AccountType == AccountType.Wealth)
            .Select(x => new WealthAccountInput
            {
                Id = Guid.NewGuid(),
                Description = x.Name,
            })
            .ToArray();

        accountInput.ExogenousAccounts = definitionList
            .Where(x => x.AccountType == AccountType.Exogenous)
            .Select(x => new ExogenousAccountInput
            {
                Id = Guid.NewGuid(),
                Description = x.Name,
            })
            .ToArray();

        accountInput.ThirdPillarAccounts = definitionList
            .Where(x => x.AccountType == AccountType.ThirdPillar)
            .Select(x => new ThirdPillarAccountInput
            {
                Id = Guid.NewGuid(),
                Description = x.Name,
            })
            .ToArray();

        accountInput.OccupationalPensionAccounts = definitionList
            .Where(x => x.AccountType == AccountType.OccupationalPension)
            .Select(x => new OccupationalPensionAccountInput
            {
                Id = Guid.NewGuid(),
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
        using SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        byte[] truncatedHashBytes = new byte[16];
        Array.Copy(hashBytes, truncatedHashBytes, 16);

        return new Guid(truncatedHashBytes);
    }
}
