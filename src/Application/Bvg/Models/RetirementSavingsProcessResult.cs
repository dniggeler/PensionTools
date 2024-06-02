using Domain.Models.Bvg;

namespace Application.Bvg.Models;

public record RetirementSavingsProcessResult(
    DateTime DateOfCalculation,
    int BvgAge,
    TechnicalAge TechnicalAge,
    decimal ProRatedFactor,
    decimal GrossInterestRate,
    decimal RetirementCredit,
    decimal RetirementCapitalWithoutInterest,
    decimal RetirementCapital,
    bool IsRetirementDate,
    bool IsEndOfSavings,
    bool IsFullYear,
    bool IsFullAge);
