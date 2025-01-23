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

builder.Services.AddCors(options => {
    options.AddPolicy("_myAllowSpecificOrigins", policy =>
        {
            policy.WithOrigins("https://localhost", "https://localhost:57276", "http://localhost:57276")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Add services to the container.
var app = builder.Build();

app.MapGraphQL();

app.UseHttpsRedirection();
app.UseCors("_myAllowSpecificOrigins");

app.Run();
