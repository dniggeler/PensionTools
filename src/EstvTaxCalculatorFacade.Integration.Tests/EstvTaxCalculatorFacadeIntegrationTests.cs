using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PensionCoach.Tools.EstvTaxCalculators;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions.Models;
using Snapshooter.Xunit;
using Xunit;

namespace EstvTaxCalculatorFacade.Integration.Tests;

public class EstvTaxCalculatorFacadeIntegrationTests
{
    const string EstvTaxCalculatorBaseUrl = "https://swisstaxcalculator.estv.admin.ch/delegate/ost-integration/v1/lg-proxy/operation/c3b67379_ESTV/";
    private readonly ServiceProvider provider;

    public EstvTaxCalculatorFacadeIntegrationTests()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddEstvTaxCalculatorClient(EstvTaxCalculatorBaseUrl);

        provider =services.BuildServiceProvider();
    }

    [Fact(DisplayName = "Tax Location")]
    public async Task SShould_Get_TaxLocation_By_Estv_Successfully()
    {
        // given
        string zip = "3303";
        string city = "Zuzwil";

        IEstvTaxCalculatorClient estvClient = provider.GetRequiredService<IEstvTaxCalculatorClient>();

        var result = await estvClient.GetTaxLocationsAsync(zip, city);

        Snapshot.Match(result);
    }


    [Fact(DisplayName = "Simple Tax")]
    public async Task Should_Calculate_Simple_Tax_Successfully()
    {
        // given

        IEstvTaxCalculatorClient estvClient = provider.GetRequiredService<IEstvTaxCalculatorClient>();

        var result = await estvClient.CalculateIncomeAndWealthTaxAsync(GetRequest());

        Snapshot.Match(result);
    }

    private static SimpleTaxRequest GetRequest()
    {
        return new SimpleTaxRequest
        {
            Children = Array.Empty<ChildModel>(),
            Confession1 = 2,
            Confession2 = 0,
            Relationship = 1,
            TaxableFortune = 500_000,
            TaxableIncomeCanton = 100_000,
            TaxableIncomeFed = 100_0000,
            TaxLocationId = 741800000,
            TaxYear = 2021
        };
    }
}
