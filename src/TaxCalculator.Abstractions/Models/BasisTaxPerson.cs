using System.Runtime.InteropServices;
using LanguageExt;


namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models
{
    public class BasisTaxPerson
    {
        public string Name { get; set; }
        public string Canton { get; set; }
        public Option<CivilStatus> CivilStatus { get; set; }
        public decimal TaxableAmount { get; set; } 
    }
}