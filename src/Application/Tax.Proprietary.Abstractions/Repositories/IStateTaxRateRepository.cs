using Domain.Models.Tax;

namespace Application.Tax.Proprietary.Abstractions;

public interface IStateTaxRateRepository
{
    TaxRateEntity TaxRates(int calculationYear, int municipalityId);
}
