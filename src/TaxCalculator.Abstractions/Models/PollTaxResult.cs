using LanguageExt;

namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models
{
    public class PollTaxResult
    {
        public Option<decimal> CantonTaxAmount { get; set; }

        public Option<decimal> MunicipalityTaxAmount { get; set; }
    }
}
