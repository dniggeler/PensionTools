using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var bvgRevision = builder.AddProject<Projects.BlazorBvgRevisionApp>("bvgrevision");
var apiService = builder.AddProject<TaxCalculator_WebApi>("apiservice");

builder
    .AddProject<BlazorApp>("webfrontend")
    .WithReference(bvgRevision)
    .WithReference(apiService)
    .WithExternalHttpEndpoints();

builder.Build().Run();
