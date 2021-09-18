using LanguageExt;
using PensionCoach.Tools.CommonTypes;

namespace Calculators.CashFlow.Models
{
    public record MultiPeriodCalculatorPerson
    {
        public string Name { get; set; }

        public Canton Canton { get; set; }

        public int MunicipalityId { get; set; }
        
        public Option<CivilStatus> CivilStatus { get; set; }
        
        public decimal Income { get; set; }
        
        public decimal Wealth { get; set; }

        public (decimal Pillar3a, decimal PensionPlan) CapitalBenefits { get; set; }
        
        public int NumberOfChildren { get; set; }
        
        public Option<ReligiousGroupType> ReligiousGroupType { get; set; }

        public Option<ReligiousGroupType> PartnerReligiousGroupType { get; set; }
    }
}
