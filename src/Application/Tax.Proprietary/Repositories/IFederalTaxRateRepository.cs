using Application.Tax.Proprietary.Enums;
using Domain.Models.Tax;

namespace Application.Tax.Proprietary.Repositories;

public interface IFederalTaxRateRepository
{
    IEnumerable<FederalTaxTariffModel> TaxRates(int calculationYear, TariffType typeId);
}
