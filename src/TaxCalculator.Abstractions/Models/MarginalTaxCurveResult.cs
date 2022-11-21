using System.Collections.Generic;
using PensionCoach.Tools.CommonTypes.Tax;

namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models;

public class MarginalTaxCurveResult
{
    public MarginalTaxRate CurrentMarginalTaxRate { get; set; }

    public Dictionary<decimal, decimal> MarginalTaxCurve { get; set; } = new();
}
