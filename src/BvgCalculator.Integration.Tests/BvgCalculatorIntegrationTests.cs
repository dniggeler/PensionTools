using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models.Bvg;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using PensionCoach.Tools.CommonTypes.Bvg;
using Snapshooter.Xunit;
using TaxCalculator.WebApi;
using Xunit;

namespace BvgCalculator.Integration.Tests;

[Trait("BVG", "Integration")]
public class BvgCalculatorIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly HttpClient client;

    public BvgCalculatorIntegrationTests(WebApplicationFactory<Startup> factory)
    {
        client = factory.CreateDefaultClient(new Uri("http://localhost/api/calculator/bvg/"));
    }

    [Fact(DisplayName = "Default")]
    public async Task ShouldCalculateSuccessfully()
    {
        var request = GetBvgRequest();

        var response = await client.PostAsJsonAsync("benefits", request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();

        Snapshot.Match(result);
    }

    [Theory(DisplayName = "Pension")]
    [InlineData(100_000, 8966, 2020, "1974-08-31", 14698)]
    [InlineData(207154, 137091.90, 2021, "1969-03-17", 19925)]
    public async Task ShouldCalculateSuccessfullyPension(
        decimal salary,
        decimal retirementCapitalEndOfYear,
        int calculationYear,
        string dateOfBirthStr,
        decimal expectedPension)
    {
        var request = GetBvgRequest();
        request.Salary = salary;
        request.RetirementCapitalEndOfYear = retirementCapitalEndOfYear;
        request.CalculationYear = calculationYear;
        request.DateOfBirth = DateTime.Parse(dateOfBirthStr);

        var response =
            await client.PostAsJsonAsync("benefits", request);

        response.EnsureSuccessStatusCode();

        BvgCalculationResult result = await response.Content.ReadFromJsonAsync<BvgCalculationResult>();

        result.RetirementPension.Should().Be(expectedPension);
    }

    private static BvgCalculationRequest GetBvgRequest()
    {
        return new BvgCalculationRequest
        {
            Name = "Test BVG",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1974,8, 31),
            CalculationYear = 2020,
            RetirementCapitalEndOfYear = 176_000,
            Salary = 45000
        };
    }
}
