using Application.Tax.Proprietary.Abstractions.Models;

namespace Application.Tax.Proprietary.Models;

public class AggregatedBasisTaxResult
{
    public BasisTaxResult IncomeTax { get; set; }
    public BasisTaxResult WealthTax { get; set; }
    public decimal Total => IncomeTax.TaxAmount + WealthTax.TaxAmount;
}
