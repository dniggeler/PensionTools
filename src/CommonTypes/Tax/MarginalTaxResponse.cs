using System.Collections.Generic;
using Domain.Models.Tax;

namespace PensionCoach.Tools.CommonTypes.Tax;

public class MarginalTaxResponse
{
    public MarginalTaxInfo CurrentMarginalTaxRate { get; set; }

    public IList<MarginalTaxInfo> MarginalTaxCurve { get; set; } = new List<MarginalTaxInfo>();
}
