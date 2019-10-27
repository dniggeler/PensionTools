using System.Runtime.InteropServices;
using LanguageExt;


namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models
{
    public class TaxPerson
    {
        public int CalculationYear { get; set; }
        public string Name { get; set; }
        public string Municipality { get; set; }
        public string Canton { get; set; }
        public Option<CivilStatus> CivilStatus { get; set; }
        public decimal TaxableIncome { get; set; } 
        public decimal TaxableFederalIncome { get; set; } 
        public decimal TaxableWealth { get; set; }
        public Option<ReligiousGroupType> DenominationType { get; set; }
    }
}