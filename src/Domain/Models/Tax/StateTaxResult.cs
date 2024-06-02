using Application.Tax.Proprietary.Abstractions.Models;

namespace Domain.Models.Tax
{
    public class StateTaxResult
    {
        public BasisTaxResult BasisIncomeTax { get; set; }

        public BasisTaxResult BasisWealthTax { get; set; }

        public ChurchTaxResult ChurchTax { get; set; }

        public decimal CantonRate { get; set; }

        public decimal MunicipalityRate { get; set; }

        public decimal MunicipalityTaxAmount => MunicipalityRate / 100M * (BasisIncomeTax.TaxAmount + BasisWealthTax.TaxAmount);
        public decimal CantonTaxAmount => CantonRate / 100M * (BasisIncomeTax.TaxAmount + BasisWealthTax.TaxAmount);

        public decimal ChurchTaxAmount => (ChurchTax.TaxAmount ?? 0) +
                                          (ChurchTax.TaxAmountPartner ?? 0);

        public decimal TotalWealthTax => BasisWealthTax.TaxAmount * (MunicipalityRate / 100M + 1);

        public decimal TotalIncomeTax => BasisIncomeTax.TaxAmount * (MunicipalityRate / 100M + 1) + ChurchTaxAmount;

        public decimal TotalTaxAmount => MunicipalityTaxAmount +
            CantonTaxAmount +
            ChurchTaxAmount +
            PollTaxAmount ?? 0m;
        public decimal? PollTaxAmount { get; set; }
    }
}
