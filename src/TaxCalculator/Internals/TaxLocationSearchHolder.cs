using PensionCoach.Tools.EstvTaxCalculators.Models;
using Tax.Data.Abstractions.Models;

namespace TaxCalculator.Internals
{
    internal record TaxLocationSearchHolder
    {
        public MunicipalityEntity MunicipalityEntity { get; init; }

        public TaxLocation[] TaxLocations { get; init; }

        public int SearchLevel { get; init; }

        public SearchResultType SearchResultType { get; init; }
    }
}
