using LanguageExt;

namespace Application.Tax.Proprietary.Abstractions.Models;

public class PollTaxResult
{
    public Option<decimal> CantonTaxAmount { get; set; }

    public Option<decimal> MunicipalityTaxAmount { get; set; }
}
