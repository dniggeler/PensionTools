using Domain.Models.Tax;

namespace Application.Tax.Proprietary.Abstractions.Repositories
{
    public interface ITaxTariffRepository
    {
        IReadOnlyCollection<TaxTariffModel> Get(TaxFilterModel filter);
    }
}
