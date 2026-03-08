using Application.Features.ContributionCalculator.Models;
using LanguageExt;
using PensionCoach.Tools.CommonUtils;

namespace Application.Features.ContributionCalculator;

public class CompoundingContributionCalculator : ICompoundingContributionCalculator
{
    public Either<string, ContributionResult> CalculateFixedYearlyContribution(
        DateTime startDate,
        decimal initialAmount,
        decimal yearlyContributionAmount,
        int contributionCount,
        bool investAtBeginningOfYear,
        decimal annualInterestRate,
        CompoundingFrequency compoundingFrequency,
        bool returnSequence = false)
    {
        if (contributionCount < 0)
        {
            return "Contribution count must be non-negative";
        }

        var contributions = Enumerable.Range(0, contributionCount)
            .Select(i =>
            {
                var date = investAtBeginningOfYear
                    ? startDate.AddYears(i)
                    : startDate.AddYears(i + 1);

                return new Contribution(date, yearlyContributionAmount);
            })
            .ToList();

        var finalDate = startDate.AddYears(contributionCount);

        var request = new ContributionRequest(
            startDate,
            initialAmount,
            contributions,
            annualInterestRate,
            finalDate,
            compoundingFrequency,
            returnSequence);

        return Calculate(request);
    }

    public Either<string, ContributionResult> CalculateYearlyContributionUntilFinalDate(
        DateTime startDate,
        DateTime finalDate,
        decimal initialAmount,
        decimal yearlyContributionAmount,
        bool investAtBeginningOfYear,
        decimal annualInterestRate,
        CompoundingFrequency compoundingFrequency,
        bool returnSequence = false)
    {
        var contributions = new List<Contribution>();
        var firstContributionDate = investAtBeginningOfYear
            ? startDate
            : startDate.AddYears(1);

        var inclusiveFinalDate = !investAtBeginningOfYear;
        for (var date = firstContributionDate;
             inclusiveFinalDate ? date <= finalDate : date < finalDate;
             date = date.AddYears(1))
        {
            contributions.Add(new Contribution(date, yearlyContributionAmount));
        }

        var request = new ContributionRequest(
            startDate,
            initialAmount,
            contributions,
            annualInterestRate,
            finalDate,
            compoundingFrequency,
            returnSequence);

        return Calculate(request);
    }

    public Either<string, ContributionResult> Calculate(ContributionRequest request)
    {
        if (request.FinalDate < request.StartDate)
        {
            return "Final date must be after start date";
        }

        var sortedContributions = request.Contributions
            .Where(c => c.Date >= request.StartDate && c.Date <= request.FinalDate)
            .OrderBy(c => c.Date)
            .ToList();

        decimal currentBalance = request.InitialAmount;
        DateTime lastDate = request.StartDate;
        var sequence = new List<ProjectionData>();

        // Initial state
        if (request.ReturnSequence)
        {
            sequence.Add(new ProjectionData(request.StartDate, currentBalance, request.InitialAmount, 0));
        }

        foreach (var contribution in sortedContributions)
        {
            if (contribution.Date > lastDate)
            {
                decimal interest = CalculateInterest(currentBalance, request.AnnualInterestRate, lastDate, contribution.Date, request.CompoundingFrequency);
                currentBalance += interest;
                
                // Add interest accrual step? Or just combined?
                // For sequence, usually we show the state *after* the transaction, including interest accrued since last time.
                // Or maybe split interest and contribution?
                // The ProjectionData has InterestAccrued field.
                
                currentBalance += contribution.Amount;
                
                if (request.ReturnSequence)
                {
                    sequence.Add(new ProjectionData(contribution.Date, currentBalance, contribution.Amount, interest));
                }

                lastDate = contribution.Date;
            }
            else
            {
                // Same date as last, just add amount
                currentBalance += contribution.Amount;
                // Update last sequence entry if exists?
                if (request.ReturnSequence && sequence.Any())
                {
                    var last = sequence.Last();
                    if (last.Date == contribution.Date)
                    {
                        // Replace last entry with updated balance and cashflow
                        sequence.RemoveAt(sequence.Count - 1);
                        sequence.Add(last with { Balance = currentBalance, CashFlow = last.CashFlow + contribution.Amount });
                    }
                    else
                    {
                        sequence.Add(new ProjectionData(contribution.Date, currentBalance, contribution.Amount, 0));
                    }
                }
            }
        }

        // Final accrual
        if (lastDate < request.FinalDate)
        {
            decimal interest = CalculateInterest(currentBalance, request.AnnualInterestRate, lastDate, request.FinalDate, request.CompoundingFrequency);
            currentBalance += interest;

            if (request.ReturnSequence)
            {
                sequence.Add(new ProjectionData(request.FinalDate, currentBalance, 0, interest));
            }
        }

        return new ContributionResult(currentBalance, request.ReturnSequence ? sequence : null);
    }

    private decimal CalculateInterest(decimal principal, decimal annualRate, DateTime from, DateTime to, CompoundingFrequency frequency)
    {
        decimal years = DateUtils.YearsBetween(from, to);
        if (years <= 0) return 0;

        decimal factor = 0;
        
        switch (frequency)
        {
            case CompoundingFrequency.Annual:
                factor = (decimal)Math.Pow((double)(1 + annualRate), (double)years);
                break;
            case CompoundingFrequency.SemiAnnual:
                factor = (decimal)Math.Pow((double)(1 + annualRate / 2m), (double)(years * 2m));
                break;
            case CompoundingFrequency.Quarterly:
                factor = (decimal)Math.Pow((double)(1 + annualRate / 4m), (double)(years * 4m));
                break;
            case CompoundingFrequency.Monthly:
                factor = (decimal)Math.Pow((double)(1 + annualRate / 12m), (double)(years * 12m));
                break;
            case CompoundingFrequency.Daily:
                factor = (decimal)Math.Pow((double)(1 + annualRate / 360m), (double)(years * 360m));
                break;
            case CompoundingFrequency.Continuous:
                factor = (decimal)Math.Exp((double)(annualRate * years));
                break;
            default:
                factor = (decimal)Math.Pow((double)(1 + annualRate), (double)years);
                break;
        }

        return principal * (factor - 1);
    }
}
