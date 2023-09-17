using Domain.Enums;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;

namespace BlazorApp.MyComponents
{
    public static class DefaultTaxPerson
    {
        public static TaxPerson GetDefaultValues() => new()
        {
            Name = "Toni",
            CivilStatus = CivilStatus.Single,
            NumberOfChildren = 0,
            ReligiousGroupType = ReligiousGroupType.Protestant,
            PartnerReligiousGroupType = null,
            TaxableIncome = 100_000,
            TaxableFederalIncome = 100_000,
            TaxableWealth = 500_000
        };
    }
}
