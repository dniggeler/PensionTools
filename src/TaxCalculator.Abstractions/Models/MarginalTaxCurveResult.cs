using System.Collections.Generic;
using PensionCoach.Tools.CommonTypes.Tax;

namespace PensionCoach.Tools.TaxCalculator.Abstractions.Models;

public class MarginalTaxCurveResult
{
    public MarginalTaxInfo CurrentMarginalTaxRate { get; set; }

    public IList<MarginalTaxInfo> MarginalTaxCurve { get; set; } = new List<MarginalTaxInfo>();
}
