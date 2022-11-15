using System.Collections.Generic;

namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models;

public class MarginalTaxCurveResult
{ 
    public Dictionary<int, decimal> MarginalTaxCurve { get; set; } = new();
}
