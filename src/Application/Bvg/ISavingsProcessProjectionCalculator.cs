using Application.Bvg.Models;
using Domain.Models.Bvg;

namespace Application.Bvg;

public interface ISavingsProcessProjectionCalculator
{
    RetirementSavingsProcessResult[] ProjectionTable(
        decimal projectionInterestRate,
        DateTime dateOfRetirement,
        DateTime dateOfEndOfSavings,
        TechnicalAge retirementAge,
        TechnicalAge finalAge,
        int yearOfBeginProjection,
        decimal beginOfRetirementCapital,
        Func<TechnicalAge, decimal> retirementCreditGetter);
}
