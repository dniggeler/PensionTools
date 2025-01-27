using Calculator;
using Microsoft.Extensions.DependencyInjection;
using StrawberryShake;

IServiceCollection serviceCollection = new ServiceCollection();

serviceCollection.AddGraphClient().ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7173/graphql"));

IServiceProvider provider = serviceCollection.BuildServiceProvider();

GraphClient? graphClient = provider.GetService<GraphClient>();

if(graphClient == null)
{
    throw new Exception("GraphClient not found");
}

Guid calculationId = Guid.NewGuid();

CalculationParametersInput calculationParameters = new()
{
    StartDate = new DateTime(2024, 1, 1),
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

IOperationResult<ICalculateResult> response = await graphClient.Calculate.ExecuteAsync(calculationId, calculationParameters, municipality, person);

response.EnsureNoErrors();

ICalculate_Calculate calculateResult = response.Data!.Calculate;

Console.WriteLine("Wealth Accounts");
foreach (var accounts in calculateResult.Transactions?.WealthAccounts ?? [])
{
    Console.WriteLine($"Account Id: {accounts.Id}");
    Console.WriteLine($"Name:       {accounts.Name}");
    foreach (var transaction in accounts.Transactions ?? [])
    {
        Console.WriteLine($"Transaction Id: {transaction?.Description}");
        Console.WriteLine($"Date:           {transaction?.ValutaDate}");
        Console.WriteLine($"Amount:         {transaction?.Flow}");
        Console.WriteLine($"Type:           {transaction?.Amount}");
    }
}
Console.WriteLine();

Console.WriteLine("Income Accounts");
foreach (var accounts in calculateResult.Transactions?.IncomeAccounts ?? [])
{
    Console.WriteLine($"Account Id: {accounts.Id}");
    Console.WriteLine($"Name:       {accounts.Name}");
    foreach (var transaction in accounts.Transactions ?? [])
    {
        Console.WriteLine($"Transaction Id: {transaction?.Description}");
        Console.WriteLine($"Date:           {transaction?.ValutaDate}");
        Console.WriteLine($"Amount:         {transaction?.Flow}");
        Console.WriteLine($"Type:           {transaction?.Amount}");
    }
}
Console.WriteLine();

Console.WriteLine("Third Pillar Accounts");
foreach (var accounts in calculateResult.Transactions?.ThirdPillarAccounts ?? [])
{
    Console.WriteLine($"Account Id: {accounts.Id}");
    Console.WriteLine($"Name:       {accounts.Name}");
    foreach (var transaction in accounts.Transactions ?? [])
    {
        Console.WriteLine($"Transaction Id: {transaction?.Description}");
        Console.WriteLine($"Date:           {transaction?.ValutaDate}");
        Console.WriteLine($"Amount:         {transaction?.Flow}");
        Console.WriteLine($"Type:           {transaction?.Amount}");
    }
}

