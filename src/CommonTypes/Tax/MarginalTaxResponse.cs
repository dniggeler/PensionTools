using System.Collections.Generic;

namespace PensionCoach.Tools.CommonTypes.Tax;

public class MarginalTaxResponse
{
    public Dictionary<int, decimal> MarginalTaxCurve { get; set; } = new();
}
