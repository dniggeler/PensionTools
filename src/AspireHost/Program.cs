var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.TaxCalculator_WebApi>("apiservice");
builder
    .AddProject<Projects.BlazorApp>("webfrontend")
    .WithReference(apiService);

builder.Build().Run();
