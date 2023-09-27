using Domain.Models.Tax;

namespace PensionCoach.Tools.CommonTypes.Features.PensionVersusCapital;

public record PensionVersusCapitalRequest
{
    public int CalculationYear { get; set; }
    public int MunicipalityId { get; set; }
    public decimal RetirementPension { get; set; }
    public decimal RetirementCapital { get; set; }

    public decimal YearlyConsumptionAmount { get; set; }

    public decimal NetWealthReturn { get; set; }

    public TaxPerson TaxPerson { get; set; }
}
