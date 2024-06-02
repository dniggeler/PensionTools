using Application.Bvg.Models;
using Domain.Models.Bvg;
using LanguageExt;

namespace Application.Bvg;

public interface IBvgCalculator
{
    Either<string, BvgCalculationResult> Calculate(int calculationYear, decimal retirementCapitalEndOfYear, BvgPerson person);

    Either<string, decimal> InsuredSalary(int calculationYear, BvgPerson person);

    Either<string, BvgTimeSeriesPoint[]> InsuredSalaries(int calculationYear, BvgPerson person);
    
    Either<string, BvgTimeSeriesPoint[]> RetirementCreditFactors(int calculationYear, BvgPerson person);
    
    Either<string, BvgTimeSeriesPoint[]> RetirementCredits(int calculationYear, BvgPerson person);
}
