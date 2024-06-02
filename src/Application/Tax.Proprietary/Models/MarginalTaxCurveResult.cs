using Domain.Models.Tax;

namespace Application.Tax.Proprietary.Models;

public class MarginalTaxCurveResult
{
    public MarginalTaxInfo CurrentMarginalTaxRate { get; set; }

    public IList<MarginalTaxInfo> MarginalTaxCurve { get; set; } = new List<MarginalTaxInfo>();
}
