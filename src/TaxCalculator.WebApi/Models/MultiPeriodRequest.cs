using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Calculators.CashFlow.Models;
using PensionCoach.Tools.CommonTypes;

namespace TaxCalculator.WebApi.Models
{
    public class MultiPeriodRequest
    {
        [MaxLength(50)]
        [NotNull]
        public string Name { get; set; }

        [Range(2018, 2099, ErrorMessage = "Valid tax years start from 2018")]
        public int StartingYear { get; set; }

        [Range(1, 100, ErrorMessage = "Number of periods to simulate")]
        public int NumberOfPeriods { get; set; }

        public CivilStatus CivilStatus { get; set; }

        public ReligiousGroupType ReligiousGroupType { get; set; }

        public ReligiousGroupType? PartnerReligiousGroupType { get; set; }

        [Range(typeof(int), "0", "100000", ErrorMessage = "BFS Number not valid")]
        public int BfsMunicipalityId { get; set; }

        [Range(0, 500_000_000, ErrorMessage = "Current taxable salary (before tax)")]
        public decimal Income { get; set; }

        [Range(0, 10_000_000_000_000, ErrorMessage = "Current wealth (before tax)")]
        public decimal Wealth { get; set; }

        [Range(0, 1_000_000, ErrorMessage = "Currently owned total capital in 3a accounts")]
        public decimal CapitalBenefitsPillar3A { get; set; }

        [Range(0, 1_000_000, ErrorMessage = "Currently owned total capital in pension plan")]
        public decimal CapitalBenefitsPension { get; set; }

        public CashFlowDefinitionHolder CashFlowDefinitionHolder { get; set; }
    }
}
