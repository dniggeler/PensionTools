using LanguageExt;


namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models
{
    public class ChurchTaxResult
    {
        public Option<decimal> TaxAmount { get; set; }

        public Option<decimal> TaxAmountPartner { get; set; }

        public decimal TaxRate { get; set; }
    }
}