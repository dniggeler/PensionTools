using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<TaxCalculator_WebApi>("apiservice");
var cashFlowEngineService = builder.AddProject<CashFlowCalculationEngine_Server>("cashflow-engine");

builder
    .AddProject<BlazorApp>("webfrontend")
    .WithReference(apiService)
    .WithReference(cashFlowEngineService)
    .WithExternalHttpEndpoints();

builder.Build().Run();
