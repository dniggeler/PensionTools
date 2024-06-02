using Application.Tax.Estv.Client.Models;
using Domain.Models.Municipality;

namespace Infrastructure.Tax.Data.Populate
{
    public record TaxLocationSearchHolder
    {
        public MunicipalityEntity MunicipalityEntity { get; init; }

        public TaxLocation[] TaxLocations { get; init; }

        public int SearchLevel { get; set; }

        public SearchResultType SearchResultType { get; set; }
    }
}
