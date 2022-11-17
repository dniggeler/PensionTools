using System.Collections.Generic;

namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models;

public class MarginalTaxCurveResult
{
    public record MarginalTaxRate(decimal Amount, decimal Rate);

    public MarginalTaxRate CurrentMarginalTaxRate { get; set; }

    public Dictionary<decimal, decimal> MarginalTaxCurve { get; set; } = new();
}
