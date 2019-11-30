using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace TaxCalculator.WebApi.Models
{
    public class FullTaxRequest
    {
        public string Name { get; set; }

        public int CalculationYear { get; set; }

        public string Canton { get; set; }

        public CivilStatus CivilStatus { get; set; }

        public ReligiousGroupType ReligiousGroup { get; set; }

        public ReligiousGroupType? PartnerReligiousGroup { get; set; }

        public string Municipality { get; set; }

        public decimal TaxableIncome { get; set; }

        public decimal TaxableFederalIncome { get; set; }

        public decimal TaxableWealth { get; set; }
    }
}