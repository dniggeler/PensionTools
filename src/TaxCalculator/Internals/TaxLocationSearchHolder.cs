using Infrastructure.EstvTaxCalculator.Client.Models;
using Tax.Data.Abstractions.Models;

namespace PensionCoach.Tools.TaxCalculator.Internals;

internal record TaxLocationSearchHolder
{
    public MunicipalityEntity MunicipalityEntity { get; init; }

    public TaxLocation[] TaxLocations { get; init; }

    public int SearchLevel { get; set; }

    public SearchResultType SearchResultType { get; set; }
}
