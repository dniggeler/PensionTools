using Domain.Enums;
using Domain.Models.Tax;

namespace PensionCoach.Tools.CommonTypes.Features.PensionVersusCapital;

public record PensionVersusCapitalRequest
{
    public int CalculationYear { get; init; }
    public int MunicipalityId { get; init; }
    public Canton Canton { get; init; }
    public decimal RetirementPension { get; init; }
    public decimal RetirementCapital { get; init; }
    public TaxPerson TaxPerson { get; init; }
}
