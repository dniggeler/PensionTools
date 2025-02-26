using Application.MultiPeriodCalculator.ExcelReader;
using Calculator;
using Microsoft.Extensions.DependencyInjection;
using MultiPeriodGraphClient;
using StrawberryShake;
using AccountType = Domain.Enums.AccountType;

IServiceCollection serviceCollection = new ServiceCollection();

serviceCollection.AddCashFlowGraphClient().ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7173/graphql"));

IServiceProvider provider = serviceCollection.BuildServiceProvider();

var graphClient = provider.GetService<CashFlowGraphClient>();

if(graphClient == null)
{
    throw new Exception("GraphClient not found");
}

var excelWorkbookFilename = @"C:\Users\dnigg\OneDrive\private\dev\PensionTools\mpcf.xlsx";

Guid calculationId = Guid.NewGuid();

CalculationParametersInput calculationParameters = new()
{
    StartDate = new DateTime(2023, 12, 31),
    EndDate = new DateTime(2030, 1, 1),
};

MunicipalityInput municipality = new()
{
    MunicipalityId = 134,
    TaxLocationId = 330400000,
    Canton = Canton.Zh,
};

var excelPerson = ExcelCashFlowReader.ReadPersons(excelWorkbookFilename).First();

CalculationPersonInput person = new()
{
    Id = Guid.NewGuid(),
    DateOfBirth = excelPerson.Birthdate.ToDateTime(TimeOnly.MinValue),
    Gender = (Gender)(int)excelPerson.Gender,
    CivilStatus = (CivilStatus)(int)excelPerson.CivilStatus,
    ReligiousGroupType = (ReligiousGroupType)(int)excelPerson.ReligiousGroupType,
    PartnerReligiousGroupType = excelPerson.PartnerReligiousGroupType.HasValue ? (ReligiousGroupType)(int)excelPerson.PartnerReligiousGroupType : null,
};

IEnumerable<ExcelAccount> excelAccounts = ExcelCashFlowReader.ReadAccounts(excelWorkbookFilename).ToList();

AccountInput accountInput = new AccountInput
{
    ExogenousAccounts = excelAccounts
        .Where(x => x.AccountType == AccountType.Exogenous)
        .Select(x => new ExogenousAccountInput
        {
            Id = x.AccountId,
            Description = x.Name,
        })
        .ToArray(),

    ThirdPillarAccounts = excelAccounts
        .Where(x => x.AccountType == AccountType.ThirdPillar)
        .Select(x => new ThirdPillarAccountInput
        {
            Id = x.AccountId,
            Description = x.Name,
        })
        .ToArray(),

    OccupationalPensionAccounts = excelAccounts
        .Where(x => x.AccountType == AccountType.OccupationalPension)
        .Select(x => new OccupationalPensionAccountInput
        {
            Id = x.AccountId,
            Description = x.Name,
        })
        .ToArray(),

    WealthAccounts = excelAccounts
        .Where(x => x.AccountType == AccountType.Wealth)
        .Select(x => new WealthAccountInput
        {
            Id = x.AccountId,
            Description = x.Name,
        })
        .ToArray(),

    InvestmentAccounts = excelAccounts
        .Where(x => x.AccountType == AccountType.Investment)
        .Select(x => new InvestmentAccountInput { Id = x.AccountId, Description = x.Name, })
        .ToArray(),

    IncomeAccounts = excelAccounts
        .Where(x => x.AccountType == AccountType.Income)
        .Select(x => new IncomeAccountInput { Id = x.AccountId, Description = x.Name, })
        .ToArray(),

    LiabilityAccounts = [],
};

var excelFixAmountCashFlows = ExcelCashFlowReader.ReadCashFlows(excelWorkbookFilename);

CashFlowInput cashFlowInput = new CashFlowInput
{
    FixedAmountCashFlows = excelFixAmountCashFlows
        .Select(a => new FixedAmountCashFlowInput
        {
            SourceAccountId = a.DebitAccountId,
            TargetAccountId = a.CreditAccountId,
            DateOfProcess = a.ProcessDate.ToDateTime(TimeOnly.MinValue),
            Description = a.Description,
            Amount = a.Amount,
            TaxType = (TaxType)(int)a.TaxType,
            TaxFlowType = (FlowType)(int)a.FlowType
        }).ToList(),
    BalanceGrowthCashFlows = [],
    TransferRatioCashFlows = [],
};


IOperationResult<ICalculateResult> response = await graphClient.Calculate.ExecuteAsync(
    calculationId,
    calculationParameters,
    municipality,
    person,
    accountInput,
    cashFlowInput);

response.EnsureNoErrors();

ICalculate_Calculate calculateResult = response.Data!.Calculate;

Console.WriteLine("Exogenous Accounts");
foreach (var accounts in calculateResult.Transactions?.ExogenousAccounts ?? [])
{
    Console.WriteLine($"Account Id: {accounts.Id,-40} Name: {accounts.Name}");
    foreach (var transaction in accounts.Transactions ?? [])
    {
        Console.WriteLine(
            $"    Description:    {transaction?.Description,-55} " +
            $"Date: {transaction?.ValutaDate,-15:yyyy-MM-dd} " +
            $"Amount: {transaction?.Flow,-10} " +
            $"Type: {transaction?.Amount,-10}");
    }
}
Console.WriteLine();
Console.WriteLine();

Console.WriteLine("Wealth Accounts");
foreach (var accounts in calculateResult.Transactions?.WealthAccounts ?? [])
{
    Console.WriteLine($"Account Id: {accounts.Id,-40} Name: {accounts.Name}");
    foreach (var transaction in accounts.Transactions ?? [])
    {
        Console.WriteLine(
            $"    Description:    {transaction?.Description,-55} " +
            $"Date: {transaction?.ValutaDate,-15:yyyy-MM-dd} " +
            $"Type: {transaction?.Flow,-10} " +
            $"Amount: {transaction?.Amount,-10}");
    }
}
Console.WriteLine();
Console.WriteLine();

Console.WriteLine("Income Accounts");
foreach (var accounts in calculateResult.Transactions?.IncomeAccounts ?? [])
{
    Console.WriteLine($"Account Id: {accounts.Id,-40} Name: {accounts.Name}");
    foreach (var transaction in accounts.Transactions ?? [])
    {
        Console.WriteLine(
            $"    Description:    {transaction?.Description,-55} " +
            $"Date: {transaction?.ValutaDate,-15:yyyy-MM-dd} " +
            $"Type: {transaction?.Flow,-10} " +
            $"Amount: {transaction?.Amount,-10}");
    }
}
Console.WriteLine();
Console.WriteLine();

Console.WriteLine("Third Pillar Accounts");
foreach (var accounts in calculateResult.Transactions?.ThirdPillarAccounts ?? [])
{
    Console.WriteLine($"Account Id: {accounts.Id,-40} Name: {accounts.Name}");
    foreach (var transaction in accounts.Transactions ?? [])
    {
        Console.WriteLine(
            $"    Description:    {transaction?.Description,-55} " +
            $"Date: {transaction?.ValutaDate,-15:yyyy-MM-dd} " +
            $"Type: {transaction?.Flow,-10} " +
            $"Amount: {transaction?.Amount,-10}");
    }
}

Console.WriteLine();
Console.WriteLine();

Console.WriteLine("Occupational Pension Accounts");
foreach (var accounts in calculateResult.Transactions?.OccupationalPensionAccounts ?? [])
{
    Console.WriteLine($"Account Id: {accounts.Id,-40} Name: {accounts.Name}");
    foreach (var transaction in accounts.Transactions ?? [])
    {
        Console.WriteLine(
            $"    Description:    {transaction?.Description,-55} " +
            $"Date: {transaction?.ValutaDate,-15:yyyy-MM-dd} " +
            $"Type: {transaction?.Flow,-10} " +
            $"Amount: {transaction?.Amount,-10}");
    }
}
Console.WriteLine();
Console.WriteLine();

foreach (var accounts in calculateResult.Transactions?.LiabilityAccounts ?? [])
{
    Console.WriteLine($"Account Id: {accounts.Id,-40} Name: {accounts.Name}");
    foreach (var transaction in accounts.Transactions ?? [])
    {
        Console.WriteLine(
            $"    Description:    {transaction?.Description,-55} " +
            $"Date: {transaction?.ValutaDate,-15:yyyy-MM-dd} " +
            $"Type: {transaction?.Flow,-10} " +
            $"Amount: {transaction?.Amount,-10}");
    }
}
Console.WriteLine();
Console.WriteLine();

IBalanceSheetGenerator balanceSheetGenerator = new BalanceSheetGenerator();
var balanceSheet = balanceSheetGenerator.GenerateBalanceSheet(calculationParameters.StartDate, calculationParameters.EndDate, calculateResult.Transactions);
Console.WriteLine($"Total Wealth:   {balanceSheet.Entry?.TotalWealth:N0}");
Console.WriteLine($"Total Liability: {-balanceSheet.Entry?.TotalLiability:N0}");
Console.WriteLine($"Total Säule 3a: {balanceSheet.Entry?.TotalThirdPillar:N0}");
Console.WriteLine($"Total 2.Säule:  {balanceSheet.Entry?.TotalOccupationalPension:N0}");
Console.WriteLine("------------------------");
Console.WriteLine($"Total Overall:  {balanceSheet.Entry?.Total:N0}");

