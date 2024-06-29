using Application.Extensions;
using BlazorBvgRevisionApp;
using BlazorBvgRevisionApp.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FeatureManagement;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddLocalization();
builder.Services.AddBvgCalculators();
builder.Services.AddFeatureManagement();

builder.Services.AddScoped<PensionPlanSimulator>();

await builder.Build().RunAsync();
