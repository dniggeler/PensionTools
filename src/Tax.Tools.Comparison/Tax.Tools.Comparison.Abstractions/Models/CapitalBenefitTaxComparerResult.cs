using PensionCoach.Tools.TaxCalculator.Abstractions.Models;


namespace Tax.Tools.Comparison.Abstractions.Models
{
    public class CapitalBenefitTaxComparerResult
    {
        public int MunicipalityId { get; set; }

        public string MunicipalityName { get; set; }

        public Canton Canton { get; set; }

        public FullCapitalBenefitTaxResult MunicipalityTaxResult { get; set; }
    }
}