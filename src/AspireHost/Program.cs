using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<TaxCalculator_WebApi>("apiservice");
builder
    .AddProject<BlazorApp>("webfrontend")
    .WithReference(apiService)
    .WithExternalHttpEndpoints();

builder.Build().Run();
