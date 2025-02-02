using AppAny.HotChocolate.FluentValidation;
using Application.Features.FullTaxCalculation;
using CashFlowCalculationEngine.Server.Application.Calculators;
using CashFlowCalculationEngine.Server.Application.Validators;
using CashFlowCalculationEngine.Server.Queries;
using Infrastructure.Configuration;
using Infrastructure.EstvTaxCalculator;
using Infrastructure.Tax.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

string corsPolicyName = "_myAllowSpecificOrigins";

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
});

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation();
    })
    .WithTracing(tracing =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // We want to view all traces in development
            tracing.SetSampler(new AlwaysOnSampler());
        }

        tracing.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
    });

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

builder.Services.AddTransient<MultiPeriodCalculationRequestValidator>();
builder.Services.AddTransient<IMultiPeriodCashFlowCalculator, MultiPeriodCashFlowCalculator>();

builder.Services.AddTaxCalculators(builder.Configuration.GetApplicationMode());
builder.Services.AddEstvTaxCalculatorClient(builder.Configuration);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<MultiPeriodCashFlowQueries>()
    .ModifyOptions(c => c.EnableOneOf = true)
    .AddFluentValidation();

builder.Services.AddCors(options => {
    options.AddPolicy(corsPolicyName, policy =>
        {
            policy.WithOrigins(
                    "https://localhost",
                    "https://localhost:49383",
                    "https://localhost:57276")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Add services to the container.
var app = builder.Build();

app.MapGraphQL();

app.UseHttpsRedirection();
app.UseCors(corsPolicyName);

app.Run();
