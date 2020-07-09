using System.Collections.Generic;

namespace PensionCoach.Tools.BvgCalculator.Models
{
    public class BvgCalculationResult
    {
        public decimal EffectiveSalary { get; set; }
        public decimal InsuredSalary { get; set; }
        public decimal RetirementCredit { get; set; }
        public decimal RetirementCreditFactor { get; set; }
        public decimal RetirementCapitalEndOfYear { get; set; }
        public decimal FinalRetirementCapital { get; set; }
        public decimal FinalRetirementCapitalWithoutInterest { get; set; }
        public decimal RetirementPension { get; set; }
        public decimal DisabilityPension { get; set; }
        public decimal PartnerPension { get; set; }
        public decimal OrphanPension { get; set; }
        public decimal ChildPensionForDisabled { get; set; }
        public IEnumerable<RetirementCredit> RetirementCreditSequence { get; set; }
        public IEnumerable<BvgRetirementCapital> RetirementCapitalSequence { get; set; }
    }
}
