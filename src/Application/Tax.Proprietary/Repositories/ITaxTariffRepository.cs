using Domain.Models.Tax;

namespace Application.Tax.Proprietary.Repositories;

public interface ITaxTariffRepository
{
    IReadOnlyCollection<TaxTariffModel> Get(TaxFilterModel filter);
}
