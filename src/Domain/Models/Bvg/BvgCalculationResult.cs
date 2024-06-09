namespace Domain.Models.Bvg
{
    public class BvgCalculationResult
    {
        public DateTime DateOfRetirement { get; set; }

        public TechnicalAge RetirementAge { get; set; }

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
        public IReadOnlyCollection<RetirementCredit> RetirementCreditSequence { get; set; }
        public IReadOnlyCollection<RetirementCapital> RetirementCapitalSequence { get; set; }
    }
}
