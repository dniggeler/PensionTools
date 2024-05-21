using Domain.Models.Bvg;
using Application.Bvg.Models;

namespace Application.Bvg;

public class SingleSavingsProcessProjectionCalculator : ISavingsProcessProjectionCalculator
{
    public RetirementSavingsProcessResult[] ProjectionTable(
        decimal projectionInterestRate,
        DateTime dateOfRetirement,
        DateTime dateOfEndOfSavings,
        TechnicalAge retirementAge,
        TechnicalAge finalAge,
        int yearOfBeginProjection,
        decimal beginOfRetirementCapital,
        Func<TechnicalAge, decimal> retirementCreditGetter)
    {
        // Values are only projected to final age
        // Note:
        // person might retired in between, or even before the
        // begin of the projection period.
        DateTime dateOfTechnicalBirth = dateOfRetirement
            .AddMonths(-retirementAge.Months)
            .AddYears(-retirementAge.Years);

        DateTime dateOfFinalAge = dateOfTechnicalBirth
            .AddMonths(finalAge.Months)
            .AddYears(finalAge.Years);

        DateTime startingDate = new(yearOfBeginProjection, 1, 1);

        if (startingDate > dateOfFinalAge)
        {
            return [];
        }

        List<RetirementSavingsProcessResult> results = [];

        // AGH at begin
        decimal aghoz = beginOfRetirementCapital;
        decimal aghmz = beginOfRetirementCapital;

        TechnicalAge birthdateAsAge = TechnicalAge.From(dateOfTechnicalBirth.Year, dateOfTechnicalBirth.Month);
        TechnicalAge startingAge = TechnicalAge.From(startingDate.Year, startingDate.Month) - birthdateAsAge;

        decimal startingAgsj = retirementCreditGetter(startingAge);

        RetirementSavingsProcessResult lastResult = new(
            DateOfCalculation: startingDate,
            BvgAge: startingDate.Year - dateOfTechnicalBirth.Year,
            TechnicalAge: startingAge,
            ProRatedFactor: decimal.One,
            GrossInterestRate: projectionInterestRate,
            RetirementCredit: startingAgsj,
            RetirementCapitalWithoutInterest: beginOfRetirementCapital,
            RetirementCapital: beginOfRetirementCapital,
            dateOfRetirement == startingDate,
            false,
            IsFullYear: true,
            startingAge.Months == 0);

        results.Add(lastResult);

        int offset = 1;
        TechnicalAge currentAge = startingAge;
        for (DateTime currentDate = startingDate.AddMonths(1); currentDate <= dateOfFinalAge; currentDate = currentDate.AddMonths(1))
        {
            bool isEndOfYear = offset % 12 == 0;

            currentAge += TechnicalAge.From(0,1);

            // pro-rated factor
            decimal phi = offset / 12M;

            decimal agsj = retirementCreditGetter(currentAge);

            decimal agsPhi = agsj * phi;

            decimal aghozPhi = aghoz + agsPhi;
            decimal aghmzPhi = aghmz * (1M + phi * projectionInterestRate) + agsPhi;

            lastResult = new RetirementSavingsProcessResult(
                DateOfCalculation: currentDate,
                BvgAge: currentDate.Year - dateOfTechnicalBirth.Year,
                TechnicalAge: currentAge,
                ProRatedFactor: phi,
                GrossInterestRate: projectionInterestRate,
                RetirementCredit: agsPhi,
                RetirementCapitalWithoutInterest: aghozPhi,
                RetirementCapital: aghmzPhi,
                dateOfRetirement == currentDate,
                dateOfEndOfSavings == currentDate,
                IsFullYear: isEndOfYear,
                currentAge.Months == 0);

            results.Add(lastResult);

            if (isEndOfYear)
            {
                aghoz += agsj;
                aghmz = aghmz * (1M + projectionInterestRate) + agsj;
                offset = 0;
            }

            offset++;
        }

        int releaseOffset = 1;
        aghoz = lastResult.RetirementCapitalWithoutInterest;
        aghmz = lastResult.RetirementCapital;
        for (DateTime currentDate = dateOfRetirement.AddMonths(1); currentDate <= dateOfEndOfSavings; currentDate = currentDate.AddMonths(1))
        {
            bool isEndOfYear = offset % 12 == 0;

            currentAge += TechnicalAge.From(0, 1);

            decimal phi = releaseOffset / 12M;

            decimal aghozPhi = aghoz;
            decimal aghmzPhi = aghmz * (1M + phi * projectionInterestRate);

            lastResult = new RetirementSavingsProcessResult(
                DateOfCalculation: currentDate,
                BvgAge: currentDate.Year - dateOfTechnicalBirth.Year,
                TechnicalAge: currentAge,
                ProRatedFactor: phi,
                GrossInterestRate: projectionInterestRate,
                RetirementCredit: decimal.Zero,
                RetirementCapitalWithoutInterest: aghozPhi,
                RetirementCapital: aghmzPhi,
                dateOfRetirement == currentDate,
                dateOfEndOfSavings == currentDate,
                IsFullYear: isEndOfYear,
                currentAge.Months == 0);

            results.Add(lastResult);

            if (isEndOfYear)
            {
                aghmz *= (1M + projectionInterestRate * phi);
                offset = 0;
                releaseOffset = 0;
            }

            offset++;
            releaseOffset++;
        }

        return results.ToArray();
    }
}

