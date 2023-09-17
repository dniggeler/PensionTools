using Infrastructure.EstvTaxCalculator.Client.Models;

namespace Tax.Data.Abstractions.Models.Populate;

public record TaxLocationSearchHolder
{
    public MunicipalityEntity MunicipalityEntity { get; init; }

    public TaxLocation[] TaxLocations { get; init; }

    public int SearchLevel { get; set; }

    public SearchResultType SearchResultType { get; set; }
}
