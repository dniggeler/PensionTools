using Domain.Enums;
using Domain.Models.Tax;

namespace Application.Features.TaxComparison.Models;

public class TaxComparerResponse
{
    public string Name { get; set; }

    public int MunicipalityId { get; set; }

    public string MunicipalityName { get; set; }
        
    public Canton Canton { get; set; }
        
    public int MaxSupportedTaxYear { get; set; }

    public decimal TotalTaxAmount { get; set; }

    public TaxAmountDetail TaxDetails { get; set; }

    public int TotalCount { get; set; }

    public int CountComputed { get; set; }
}
