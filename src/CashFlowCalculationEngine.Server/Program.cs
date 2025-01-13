using AppAny.HotChocolate.FluentValidation;
using Application.Features.FullTaxCalculation;
using CashFlowCalculationEngine.Server.Application.Calculators;
using CashFlowCalculationEngine.Server.Application.Validators;
using CashFlowCalculationEngine.Server.Queries;
using Infrastructure.Configuration;
using Infrastructure.EstvTaxCalculator;
using Infrastructure.Tax.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<MultiPeriodCalculationRequestValidator>();
builder.Services.AddTransient<IMultiPeriodCashFlowCalculator, MultiPeriodCashFlowCalculator>();

builder.Services.AddTaxData(builder.Configuration);
builder.Services.AddTaxCalculators(builder.Configuration.GetApplicationMode());
builder.Services.AddEstvTaxCalculatorClient(builder.Configuration);

builder.Services.AddGraphQLServer()
    .AddQueryType<MultiPeriodCashFlowQueries>()
    .ModifyOptions(c => c.EnableOneOf = true)
    .AddFluentValidation();

// Add services to the container.
var app = builder.Build();

app.MapGraphQL();

app.UseHttpsRedirection();

app.Run();
