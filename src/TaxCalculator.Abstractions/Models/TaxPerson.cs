using System.Runtime.InteropServices;
using LanguageExt;


namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models
{
    public enum CivilStatus {  Single, Married }
    public enum TariffType { Base, Married }
    public enum DenominationType { Base, Married }
    public class TaxPerson
    {
        public int CalculationYear { get; set; }
        public string Name { get; set; }
        public string Municipality { get; set; }
        public string Canton { get; set; }
        public Option<CivilStatus> CivilStatus { get; set; }
        public Option<TariffType> TariffType { get; set; }
        public decimal TaxableIncome { get; set; } 
        public Option<decimal> TaxableWealth { get; set; }
        public Option<DenominationType> DenominationType { get; set; }
    }
}