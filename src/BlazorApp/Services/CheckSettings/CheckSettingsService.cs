﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Services.CheckSettings;

public class CheckSettingsService : ICheckSettingsService
{
    private readonly IWebAssemblyHostEnvironment webAssemblyHostEnvironment;
    private readonly IConfiguration configuration;
    private readonly HttpClient httpClient;
    private readonly ILogger<TaxCalculationService> logger;

    public CheckSettingsService(
        IWebAssemblyHostEnvironment webAssemblyHostEnvironment,
        IConfiguration configuration,
        HttpClient httpClient,
        ILogger<TaxCalculationService> logger)
    {
        this.webAssemblyHostEnvironment = webAssemblyHostEnvironment;
        this.configuration = configuration;
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async Task<bool> HealthCheckAsync()
    {
        string urlPath = Path.Combine(BaseAddress(), "health");

        var response = await httpClient.GetStringAsync(urlPath);

        logger.LogInformation($"Health check response: {response}");

        return "Healthy" == response;
    }

    public Task<Dictionary<string, string>> GetFrontendConfigurationAsync()
    {
        var configs = new Dictionary<string, string>();

        configs.TryAdd("Environment", webAssemblyHostEnvironment.Environment);
        
        return Task.FromResult(configs);
    }

    public async Task<Dictionary<string, string>> GetBackendConfigurationAsync()
    {
        string urlPath = Path.Combine(BaseAddress(), "api/check/settings");

        Dictionary<string, string> response = await httpClient.GetFromJsonAsync<Dictionary<string,string>>(urlPath);

        return response;
    }

    private string BaseAddress()
    {
        string baseUri = configuration.GetSection("TaxCalculatorServiceUrl").Value;

        if (baseUri is null)
        {
            throw new ArgumentNullException(nameof(baseUri));
        }

        return new Uri(baseUri).GetLeftPart(UriPartial.Authority);
    }
}
