using Domain.Models.Tax;

namespace Domain.Contracts.Data;

public interface ITaxTariffData
{
    IReadOnlyCollection<TaxTariffModel> Get(TaxFilterModel filter);
}
