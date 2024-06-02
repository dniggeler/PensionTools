using Domain.Enums;
using Domain.Models.Tax;

namespace Domain.Models.TaxComparison
{
    public class CapitalBenefitTaxComparerResult
    {
        public int MunicipalityId { get; set; }

        public string MunicipalityName { get; set; }

        public Canton Canton { get; set; }

        public int MaxSupportedTaxYear { get; set; }

        public FullCapitalBenefitTaxResult MunicipalityTaxResult { get; set; }

        public int TotalCount { get; set; }
    }
}
