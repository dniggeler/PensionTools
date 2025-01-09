using Domain.Enums;

namespace CashFlowCalculationEngine.Server.Domain.Location;

public class Municipality
{
    public int MunicipalityId { get; set; }

    public int? TaxLocationId { get; set; }

    public string? ZipCode { get; set; }

    public string? City { get; set; }

    public Canton? Canton { get; set; }
}
