using System.Collections.Generic;
using System.Linq;
using Domain.Models.Tax;
using FluentAssertions;
using Infrastructure.Tax.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Tax.Data.Tests;

[Trait("Data", "DB Context")]
public class TaxDbContextTests : IClassFixture<TaxDataFixture>
{
    private readonly TaxDataFixture _fixture;

    public TaxDbContextTests(TaxDataFixture fixture)
    {
        _fixture = fixture;
    }


    [Fact(DisplayName = "Connection String")]
    public void ShouldReturnValidConnectionString()
    {
        // given

        // when
        var configSvc = _fixture.Provider.GetRequiredService<IConfiguration>();

        var result = configSvc.GetConnectionString("TaxDb");

        System.Diagnostics.Trace.WriteLine($"Connection String={result}");

        // then
        result.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Tax Rate")]
    public void ShouldReturnTaxRateForMunicipality()
    {
        // given
        int calculationYear = 2019;
        string canton = "ZH";

        // when
        var result = GetRates();

        // then
        result.Should().NotBeNullOrEmpty();

        IEnumerable<TaxRateEntity> GetRates()
        {
            using var dbContext = _fixture.Provider.GetService<TaxRateDbContext>();
            return dbContext.Rates.Where(item => item.Canton == canton &&
                                                 item.Year == calculationYear)
                .ToList();
        }
    }

    [Fact(DisplayName = "Federal Tax Tariff")]
    public void ShouldReturnFederalTaxTariff()
    {
        // given
        int calculationYear = 2019;

        // when
        var result = GetTariffs();

        // then
        result.Should().NotBeNullOrEmpty();

        IEnumerable<FederalTaxTariffModel> GetTariffs()
        {
            using var dbContext = _fixture.Provider.GetService<FederalTaxTariffDbContext>();
            return dbContext.Tariffs
                .Where(item => item.Year == calculationYear)
                .ToList()
                .OrderByDescending(item => item.IncomeLevel);
        }
    }

    [Fact(DisplayName = "State Tax Tariffs")]
    public void ShouldReturnTaxTariff()
    {
        // given
        int calculationYear = 2019;
        string canton = "ZH";

        // when
        var result = GetTariffs();

        // then
        result.Should().NotBeNullOrEmpty();

        IEnumerable<TaxTariffModel> GetTariffs()
        {
            using var dbContext = _fixture.Provider.GetService<TaxTariffDbContext>();
            return dbContext.Tariffs.Where(item => item.Canton == canton
                                                   && item.Year == calculationYear)
                .ToList();
        }
    }
}
