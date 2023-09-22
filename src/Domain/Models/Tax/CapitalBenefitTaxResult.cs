using Application.Tax.Proprietary.Abstractions.Models;

namespace Domain.Models.Tax;

public class CapitalBenefitTaxResult
{
    public BasisTaxResult BasisTax { get; set; }
    public ChurchTaxResult ChurchTax { get; set; }
    public decimal CantonRate { get; set; }
    public decimal MunicipalityRate { get; set; }
    public decimal MunicipalityTaxAmount => MunicipalityRate / 100M * BasisTax.TaxAmount;
    public decimal CantonTaxAmount => CantonRate / 100M * BasisTax.TaxAmount;

    public decimal ChurchTaxAmount => (ChurchTax.TaxAmount ?? 0) +
                                      (ChurchTax.TaxAmountPartner ?? 0);

    public decimal TotalTaxAmount => MunicipalityTaxAmount +
                                     CantonTaxAmount +
                                     ChurchTaxAmount;
}
