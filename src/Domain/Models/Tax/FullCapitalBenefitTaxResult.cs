﻿using Application.Tax.Proprietary.Abstractions.Models;

namespace Domain.Models.Tax;

public class FullCapitalBenefitTaxResult
{ 
    public CapitalBenefitTaxResult StateResult { get; set; }
    public BasisTaxResult FederalResult { get; set; }
    public decimal TotalTaxAmount => StateResult.TotalTaxAmount + FederalResult.TaxAmount;
}
