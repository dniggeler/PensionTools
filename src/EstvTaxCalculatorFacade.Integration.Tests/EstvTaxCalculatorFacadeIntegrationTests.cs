using System;
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

        provider = services.BuildServiceProvider();
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


    [Theory(DisplayName = "Income and Wealth Tax")]
    [InlineData(2022, 800000000, 500_000, "Married", "Other", "Other")]
    [InlineData(2022, 800000000, 500_000, "Married", "Protestant", "Other")]
    [InlineData(2022, 800000000, 0, "Married", "Other", "Other")]
    [InlineData(2022, 885300000, 500_000, "Married", "Protestant", "Other")]
    [InlineData(2022, 885300000, 500_000, "Married", "Protestant", "Roman")]
    public async Task Should_Calculate_Income_And_Wealth_Tax_Successfully(
        int taxYear, int taxLocationId, decimal wealth, string civilStatusString, string religiousType, string religiousTypePartner)
    {
        // given
        IEstvTaxCalculatorClient estvClient = provider.GetRequiredService<IEstvTaxCalculatorClient>();

        var result = await estvClient.CalculateIncomeAndWealthTaxAsync(
            taxLocationId, taxYear, GetPerson(wealth, civilStatusString, religiousType, religiousTypePartner));

        Snapshot.Match(result, $"ESTV SimpleTax {taxLocationId}{wealth}{civilStatusString}{religiousType}{religiousTypePartner}");
    }

    [Theory(DisplayName = "Capital Benefit Tax")]
    [InlineData(2022, 800000000, 1_000_000, "Married", "Other", "Other")]
    [InlineData(2022, 800000000, 1_000_000, "Married", "Protestant", "Other")]
    [InlineData(2022, 800000000, 0, "Married", "Other", "Other")]
    [InlineData(2022, 885300000, 1_000_000, "Married", "Protestant", "Other")]
    [InlineData(2022, 885300000, 1_000_000, "Married", "Protestant", "Roman")]
    public async Task Calculate_Capital_Benefit_Tax_Successfully(int taxYear, int taxLocationId, decimal capitalBenefits, string civilStatusString, string religiousType, string religiousTypePartner)
    {
        // given
        IEstvTaxCalculatorClient estvClient = provider.GetRequiredService<IEstvTaxCalculatorClient>();

        var result = await estvClient.CalculateCapitalBenefitTaxAsync(
            taxLocationId, taxYear, GetCapitalBenefitPerson(capitalBenefits, civilStatusString, religiousType, religiousTypePartner));

        Snapshot.Match(result, $"ESTV CapitalBenefits {taxLocationId}{capitalBenefits}{civilStatusString}{religiousType}{religiousTypePartner}");
    }

    private static TaxPerson GetPerson(decimal wealth, string civilStatusString, string religiousTypePerson1, string religiousTypePerson2)
    {

        TaxPerson person = new TaxPerson
        {
            Name = "Tester",
            CivilStatus = CivilStatus.Single,
            NumberOfChildren = 0,
            ReligiousGroupType = ReligiousGroupType.Other,
            TaxableWealth = wealth,
            TaxableFederalIncome = 100_000,
            TaxableIncome = 100_000,
        };


        if (Enum.TryParse<CivilStatus>(civilStatusString, out var civilStatus))
        {
            person.CivilStatus = civilStatus;
        }

        if (Enum.TryParse<ReligiousGroupType>(religiousTypePerson1, out var person1))
        {
            person.ReligiousGroupType = person1;
        }

        if (person.CivilStatus == CivilStatus.Married &&
            Enum.TryParse<ReligiousGroupType>(religiousTypePerson2, out var person2))
        {
            person.PartnerReligiousGroupType = person2;
        }

        return person;
    }

    private static CapitalBenefitTaxPerson GetCapitalBenefitPerson(
        decimal capitalBenefits, string civilStatusString, string religiousTypePerson1, string religiousTypePerson2)
    {
        var person = new CapitalBenefitTaxPerson
        {
            Name = "Tester",
            CivilStatus = CivilStatus.Single,
            NumberOfChildren = 0,
            ReligiousGroupType = ReligiousGroupType.Other,
            TaxableCapitalBenefits = capitalBenefits,
        };

        if (Enum.TryParse<CivilStatus>(civilStatusString, out var civilStatus))
        {
            person.CivilStatus = civilStatus;
        }

        if (Enum.TryParse<ReligiousGroupType>(religiousTypePerson1, out var person1))
        {
            person.ReligiousGroupType = person1;
        }

        if (person.CivilStatus == CivilStatus.Married &&
            Enum.TryParse<ReligiousGroupType>(religiousTypePerson2, out var person2))
        {
            person.PartnerReligiousGroupType = person2;
        }

        return person;
    }
}
