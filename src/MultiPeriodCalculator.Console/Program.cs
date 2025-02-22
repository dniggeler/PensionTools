using Calculator;
using Microsoft.Extensions.DependencyInjection;
using MultiPeriodCalculator.Console.Excel;
using MultiPeriodGraphClient;
using StrawberryShake;

IServiceCollection serviceCollection = new ServiceCollection();

serviceCollection.AddCashFlowGraphClient().ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7173/graphql"));

IServiceProvider provider = serviceCollection.BuildServiceProvider();

var graphClient = provider.GetService<CashFlowGraphClient>();

if(graphClient == null)
{
    throw new Exception("GraphClient not found");
}

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

CalculationPersonInput person = new()
{
    Id = Guid.NewGuid(),
    DateOfBirth = new DateTime(1980, 1, 1),
    Gender = Gender.Male,
    CivilStatus = CivilStatus.Married,
    ReligiousGroupType = ReligiousGroupType.Other,
    PartnerReligiousGroupType = ReligiousGroupType.Other
};

var excelAccounts = ExcelReader.ReadAccountInput(@"C:\Users\dnigg\OneDrive\private\dev\PensionTools\mpcf.xlsx");

AccountInput accountInput = new AccountInput
{
    ExogenousAccounts = excelAccounts.ExogenousAccounts
        .Select(a => new ExogenousAccountInput { Id = a.Id, Description = a.Description }).ToList(),
    ThirdPillarAccounts = excelAccounts.ThirdPillarAccounts
        .Select(a => new ThirdPillarAccountInput { Id = a.Id, Description = a.Description }).ToList(),
    OccupationalPensionAccounts = excelAccounts.OccupationalPensionAccounts
        .Select(a => new OccupationalPensionAccountInput { Id = a.Id, Description = a.Description }).ToList(),
    WealthAccounts = excelAccounts.WealthAccounts
        .Select(a => new WealthAccountInput { Id = a.Id, Description = a.Description }).ToList(),
    InvestmentAccounts = excelAccounts.InvestmentAccounts
        .Select(a => new InvestmentAccountInput { Id = a.Id, Description = a.Description }).ToList(),
    IncomeAccounts = excelAccounts.IncomeAccounts
        .Select(a => new IncomeAccountInput { Id = a.Id, Description = a.Description }).ToList(),
    LiabilityAccounts = excelAccounts.LiabilityAccounts
        .Select(a => new LiabilityAccountInput { Id = a.Id, Description = a.Description }).ToList(),
};


IOperationResult<ICalculateResult> response = await graphClient.Calculate.ExecuteAsync(
    calculationId,
    calculationParameters,
    municipality,
    person,
    accountInput);

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

