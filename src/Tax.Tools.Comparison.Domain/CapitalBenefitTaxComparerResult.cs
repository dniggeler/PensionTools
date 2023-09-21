using Application.Tax.Proprietary.Abstractions.Models;
using Domain.Enums;

namespace PensionCoach.Tools.TaxComparison
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
