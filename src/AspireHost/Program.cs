var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.TaxCalculator_WebApi>("apiservice");

builder.Build().Run();
