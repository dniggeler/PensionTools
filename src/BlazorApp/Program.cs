﻿using Microsoft.Extensions.DependencyInjection;
using BlazorApp.Services;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using BlazorApp.Services.Mock;
using System.Globalization;
using BlazorApp;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using System.Net.Http;
using System;
using Application.Bvg;
using Application.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
//builder.Services.AddHttpClient<TaxCalculationService>(client => client.BaseAddress = new("http://apiservice"));
//builder.Services.AddHttpClient<MunicipalityServiceClient>(client => client.BaseAddress = new("http://apiservice"));
//builder.Services.AddHttpClient<TaxScenarioService>(client => client.BaseAddress = new("http://apiservice"));
//builder.Services.AddHttpClient<MultiPeriodCalculationService>(client => client.BaseAddress = new("http://apiservice"));
//builder.Services.AddHttpClient<TaxComparisonService>(client => client.BaseAddress = new("http://apiservice"));


if (builder.HostEnvironment.IsEnvironment("Mock"))
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

builder.Services.AddBvgCalculators();

builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddLocalization();
            
if (!builder.HostEnvironment.IsProduction())
{
    builder.Services.AddLogging(b => b.SetMinimumLevel(LogLevel.Debug).AddFilter("Microsoft", LogLevel.Information));
}

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("de-CH");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("de-CH");

var app = builder.Build();

await app.RunAsync();
