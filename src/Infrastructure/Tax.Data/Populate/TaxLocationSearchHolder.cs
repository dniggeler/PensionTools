using Domain.Models.Municipality;
using Infrastructure.EstvTaxCalculator.Client.Models;

namespace Infrastructure.Tax.Data.Populate;

public record TaxLocationSearchHolder
{
    public MunicipalityEntity MunicipalityEntity { get; init; }

    public TaxLocation[] TaxLocations { get; init; }

    public int SearchLevel { get; set; }

    public SearchResultType SearchResultType { get; set; }
}
