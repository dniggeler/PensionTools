using System.Collections.Generic;
using Tax.Data.Abstractions.Models;

namespace Tax.Data.Abstractions
{
    public interface ITaxTariffData
    {
        IReadOnlyCollection<TaxTariffModel> Get(TaxFilterModel filter);
    }
}