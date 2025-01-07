using CashFlowCalculationEngine.Server.Domain.Accounts;
using CashFlowCalculationEngine.Server.Queries;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGraphQLServer()
    .AddQueryType<MultiPeriodCashFlowQueries>()
    //.AddType<GenericCashFlowAccount>()
    .AddType<ExogenousAccount>()
    .AddType<IncomeAccount>()
    .AddType<OccupationalPensionAccount>()
    .AddType<TaxAccount>()
    .AddType<ThirdPillarAccount>()
    .AddType<WealthAccount>()
    //.AddType<InvestmentAccount>()
    .ModifyOptions(c => c.EnableOneOf = true);

// Add services to the container.
var app = builder.Build();

app.MapGraphQL();

app.UseHttpsRedirection();

app.Run();
