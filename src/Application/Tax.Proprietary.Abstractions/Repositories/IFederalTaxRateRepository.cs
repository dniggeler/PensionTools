using Application.Tax.Proprietary.Abstractions.Models;
using Domain.Models.Tax;

namespace Application.Tax.Proprietary.Abstractions.Repositories
{
    public interface IFederalTaxRateRepository
    {
        IEnumerable<FederalTaxTariffModel> TaxRates(int calculationYear, TariffType typeId);
    }
}
