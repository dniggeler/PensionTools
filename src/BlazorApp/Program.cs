﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using BlazorApp.Services;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using BlazorApp.Services.Mock;
using System.Globalization;
using BlazorApp;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

//builder.Configuration.AddJsonFile("appsettings.json")
//    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true)
//    .AddEnvironmentVariables();

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<TaxCalculationService>(client => client.BaseAddress = new("http://apiservice"));
builder.Services.AddHttpClient<MunicipalityServiceClient>(client => client.BaseAddress = new("http://apiservice"));


if (builder.Environment.IsEnvironment("Mock"))
{
    builder.Services.AddScoped<IMultiPeriodCalculationService, MockedPensionToolsCalculationService>();
    builder.Services.AddScoped<ITaxCalculationService, MockedPensionToolsCalculationService>();
    builder.Services.AddScoped<IMarginalTaxCurveCalculationService, MockedPensionToolsCalculationService>();
    builder.Services.AddScoped<ITaxComparisonService, MockTaxComparisonService>();
    builder.Services.AddScoped<ITaxScenarioService, MockTaxComparisonService>();
    builder.Services.AddScoped<IMunicipalityService, MockedMunicipalityService>();

    builder.Services.AddMockServices();
}
else
{
    builder.Services.AddScoped<IMultiPeriodCalculationService, MultiPeriodCalculationService>();
    builder.Services.AddScoped<ITaxCalculationService, TaxCalculationService>();
    builder.Services.AddScoped<IMarginalTaxCurveCalculationService, TaxCalculationService>();
    builder.Services.AddScoped<ITaxComparisonService, TaxComparisonService>();
    builder.Services.AddScoped<IMunicipalityService, MunicipalityServiceClient>();
    builder.Services.AddScoped<ITaxScenarioService, TaxScenarioService>();

    builder.Services.AddServices();
}

builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddLocalization();
            
if (!builder.Environment.IsProduction())
{
    builder.Services.AddLogging(b => b.SetMinimumLevel(LogLevel.Debug).AddFilter("Microsoft", LogLevel.Information));
}

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("de-CH");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("de-CH");

var app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
