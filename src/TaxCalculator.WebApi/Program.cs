using System.Reflection;
using System;
using System.IO;
using System.Text.Json.Serialization;
using Application.Extensions;
using Application.Features.FullTaxCalculation;
using Application.Features.PensionVersusCapital;
using Application.Features.TaxComparison;
using Application.MultiPeriodCalculator;
using Aspire;
using Infrastructure.Configuration;
using Infrastructure.DataStaging;
using Infrastructure.EstvTaxCalculator;
using Infrastructure.HealthChecks;
using Infrastructure.PostOpenApi;
using Infrastructure.Tax.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using TaxCalculator.WebApi.Examples;

string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();

builder.Configuration.AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true)
    .AddEnvironmentVariables();

builder.AddServiceDefaults();

builder.Services.AddCors(options =>
    options.AddPolicy(myAllowSpecificOrigins, corsBuilder =>
    {
        corsBuilder.WithOrigins(
                "http://localhost:5167",
                "http://localhost:8080",
                "https://localhost:8080",
                "https://localhost:5001",
                "http://localhost:44331",
                "https://localhost:44331",
                "http://localhost:44353",
                "https://localhost:44353",
                "http://localhost:57094",
                "https://localhost:57094",
                "https://x14qsqjz-44331.euw.devtunnels.ms",
                "https://relaxed-bose-eb5bc2.netlify.com",
                "https://pensiontoolsblazor.z6.web.core.windows.net")
            .AllowAnyHeader()
            .AllowAnyMethod();
    }));

builder.Services.AddHealthChecks()
    .AddCheck<TaxCalculatorHealthCheck>("Tax calculator check")
    .AddDbContextCheck<FederalTaxTariffDbContext>()
    .AddDbContextCheck<TaxTariffDbContext>()
    .AddDbContextCheck<TaxRateDbContext>()
    .AddDbContextCheck<MunicipalityDbContext>();

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddTaxData(builder.Configuration);
builder.Services.AddTaxCalculators(builder.Configuration.GetApplicationMode());
builder.Services.AddTaxComparers();
builder.Services.AddDataStagingServices();
builder.Services.AddBvgCalculators();
builder.Services.AddCashFlowCalculators();
builder.Services.AddToolsCalculators();
builder.Services.AddEstvTaxCalculatorClient(builder.Configuration);
builder.Services.AddPostOpenApiClient(builder.Configuration);
builder.Services.AddSwaggerExamplesFromAssemblyOf<CapitalBenefitTaxRequestExample>();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Tax Calculators",
        Contact = new OpenApiContact
        {
            Name = "Dieter Niggeler",
            Email = "dnig69+2@gmail.com",
        },
        Version = "v1",
    });
    opt.ExampleFilters();

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    opt.IncludeXmlComments(xmlPath);
});

var app = builder.Build();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors(myAllowSpecificOrigins);
app.UseRouting();
app.UseAuthorization();
app.UseHealthChecks("/");
app.UseSwagger();
app.UseSwaggerUI(opt =>
{
    opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Tax Calculators Api V1");
    opt.RoutePrefix = "swagger";
});

app.MapControllers();
app.MapDefaultEndpoints();

app.Run();

