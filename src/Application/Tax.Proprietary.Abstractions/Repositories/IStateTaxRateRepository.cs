using Domain.Models.Tax;

namespace Application.Tax.Proprietary.Abstractions.Repositories;

public interface IStateTaxRateRepository
{
    TaxRateEntity TaxRates(int calculationYear, int municipalityId);
}
