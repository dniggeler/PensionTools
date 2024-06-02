using Domain.Models.Tax;

namespace Application.Tax.Proprietary.Repositories;

public interface IStateTaxRateRepository
{
    TaxRateEntity TaxRates(int calculationYear, int municipalityId);

    IEnumerable<TaxRateEntity> TaxRates();
}
