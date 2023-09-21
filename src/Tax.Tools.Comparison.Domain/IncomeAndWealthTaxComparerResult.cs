using Application.Tax.Proprietary.Abstractions.Models;
using Domain.Enums;

namespace PensionCoach.Tools.TaxComparison;

public class IncomeAndWealthTaxComparerResult
{
    public int MunicipalityId { get; set; }

    public string MunicipalityName { get; set; }

    public Canton Canton { get; set; }

    public int MaxSupportedTaxYear { get; set; }

    public FullTaxResult TaxResult { get; set; }

    public int TotalCount { get; set; }
}
