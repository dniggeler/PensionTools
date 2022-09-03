using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.EstvTaxCalculators;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions;
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
    public async Task Should_Get_TaxLocation_By_Estv_Successfully()
    {
        // given
        string zip = "3303";
        string city = "Zuzwil";

        IEstvTaxCalculatorClient estvClient = provider.GetRequiredService<IEstvTaxCalculatorClient>();

        var result = await estvClient.GetTaxLocationsAsync(zip, city);

        Snapshot.Match(result);
    }


    [Fact(DisplayName = "Income and Wealth Tax")]
    public async Task Should_Calculate_Income_And_Wealth_Tax_Successfully()
    {
        // given
        int taxLocationId = 800000000;
        int taxYear = 2021;

        IEstvTaxCalculatorClient estvClient = provider.GetRequiredService<IEstvTaxCalculatorClient>();

        var result = await estvClient.CalculateIncomeAndWealthTaxAsync(taxLocationId, taxYear, GetPerson());

        Snapshot.Match(result);
    }

    [Fact(DisplayName = "Capital Benefit Tax")]
    public async Task Calculate_Capital_Benefit_Tax_Successfully()
    {
        // given
        int taxLocationId = 800000000;
        int taxYear = 2021;

        IEstvTaxCalculatorClient estvClient = provider.GetRequiredService<IEstvTaxCalculatorClient>();

        var result = await estvClient.CalculateCapitalBenefitTaxAsync(taxLocationId, taxYear, GetCapitalBenefitPerson());

        Snapshot.Match(result);
    }

    private static TaxPerson GetPerson()
    {
        return new TaxPerson
        {
            Name = "Tester",
            CivilStatus = CivilStatus.Single,
            NumberOfChildren = 0,
            ReligiousGroupType = ReligiousGroupType.Roman,
            TaxableWealth = 500_000,
            TaxableFederalIncome = 100_000,
            TaxableIncome = 100_000,
        };
    }

    private static CapitalBenefitTaxPerson GetCapitalBenefitPerson()
    {
        return new CapitalBenefitTaxPerson
        {
            Name = "Tester",
            CivilStatus = CivilStatus.Single,
            NumberOfChildren = 0,
            ReligiousGroupType = ReligiousGroupType.Roman,
            TaxableCapitalBenefits = 500_000,
        };
    }
}
