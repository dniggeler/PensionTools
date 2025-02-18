using Calculator;
using Microsoft.Extensions.DependencyInjection;
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

AccountInput accountInput = new AccountInput
{
    ThirdPillarAccounts =
    [
        new ThirdPillarAccountInput { Id = Guid.Parse("f00d246d-e518-497a-8629-9adb2f2bbae0"), Description = "3a SL" },
        new ThirdPillarAccountInput { Id = Guid.Parse("f00d246d-e518-497a-8629-9adb2f2bbae1"), Description = "3a Bank" }
    ],
    OccupationalPensionAccounts = [
        new OccupationalPensionAccountInput { Id = Guid.Parse("f00d246d-e518-497b-8629-9adb2f2bbae1"), Description = "Meine PK" }
    ],
    WealthAccounts = [
        new WealthAccountInput { Id = Guid.Parse("e00d246d-e518-497a-8629-9adb2f2bbae4"), Description = "Sonstiges Vermögen" },
        new WealthAccountInput { Id = Guid.Parse("c0000000-0000-0000-8629-9adb2f2bbae4"), Description = "Kredit" }
    ],
    IncomeAccounts = [
        new IncomeAccountInput { Id = Guid.Parse("f00d246d-e518-497a-8629-9adb2f2bbae2"), Description = "Lohnkonto" }
    ],
    ExogenousAccounts = [
        new ExogenousAccountInput { Id = Guid.Parse("f0000000-e518-497a-8629-000000000000"), Description = "Kreditgeber" },
        new ExogenousAccountInput { Id = Guid.Parse("f00d246d-e518-497a-8629-9adb2f2bbae3"), Description = "Arbeitgeber" },
        new ExogenousAccountInput { Id = Guid.Parse("f10d246d-e518-497a-8629-9adb2f2bbae3"), Description = "Initialer Setup" },
        new ExogenousAccountInput { Id = Guid.Parse("b10d246d-e518-497a-8629-9adb2f2bbae3"), Description = "Steueramt" },
        new ExogenousAccountInput { Id = Guid.Parse("a10d246d-e518-497a-8629-9adb2f2bbae3"), Description = "Wachstum" }
    ],
    InvestmentAccounts = [],
    LiabilityAccounts = []
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

