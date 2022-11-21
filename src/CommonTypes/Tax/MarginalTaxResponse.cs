using System.Collections.Generic;

namespace PensionCoach.Tools.CommonTypes.Tax;

public class MarginalTaxResponse
{
    public MarginalTaxRate CurrentMarginalTaxRate { get; set; }

    public Dictionary<int, decimal> MarginalTaxCurve { get; set; } = new();
}
