using Application.Bvg.Models;
using Application.Extensions;
using Domain.Enums;
using Domain.Models.Bvg;
using FluentValidation;
using FluentValidation.Results;
using LanguageExt;
using PensionCoach.Tools.CommonUtils;

namespace Application.Bvg;

public class BvgRevisionCalculator(
    BvgCalculator bvgCalculator,
    BvgRetirementDateCalculator retirementDateCalculator,
    IBvgRetirementCredits retirementCredits,
    ISavingsProcessProjectionCalculator projectionCalculator,
    IValidator<BvgPerson> bvgPersonValidator)
    : IBvgCalculator
{
    private const decimal PensionConversionRate = 0.06M;
    private const decimal SalaryThresholdFactor = 0.675M;
    private const decimal CoordinationDeductionFactor = 0.2M;
    private static readonly DateTime StartOfBvgRevision = new(2026, 1, 1);
    
    public Either<string, BvgCalculationResult> Calculate(int calculationYear, decimal retirementCapitalEndOfYear, BvgPerson person)
    {
        Option<ValidationResult> validationResult = bvgPersonValidator.Validate(person);

        return validationResult
            .Where(r => !r.IsValid)
            .Map<Either<string, bool>>(r =>
            {
                var errorMessageLine = string.Join(";", r.Errors.Select(x => x.ErrorMessage));
                return $"validation failed: {errorMessageLine}";
            })
            .IfNone(true)
            .Bind(_ => CalculateInternal(calculationYear, retirementCapitalEndOfYear, person));
    }

    public Either<string, decimal> InsuredSalary(int calculationYear, BvgPerson person)
    {
        if(calculationYear < StartOfBvgRevision.Year)
        {
            return bvgCalculator.InsuredSalary(calculationYear, person);
        }

        bool isRetired = IsRetired(person, new DateTime(calculationYear, 1, 1));

        if (isRetired && person.DisabilityDegree == decimal.One)
        {
            return decimal.Zero;
        }

        return UnconditionedInsuredSalary(calculationYear, person.ReportedSalary, decimal.One - person.DisabilityDegree);
    }

    public Either<string, BvgTimeSeriesPoint[]> InsuredSalaries(int calculationYear, BvgPerson person)
    {
        DateTime technicalBirthdate = person.DateOfBirth.GetBirthdateTechnical();
        TechnicalAge birthdateAsAge = (technicalBirthdate.Year, technicalBirthdate.Month);
        DateTime dateOfFinalAge = GetRetirementDate(person.DateOfBirth, person.Gender);

        List<BvgTimeSeriesPoint> salaries = [];
        for (DateTime currentDate = new DateTime(calculationYear, 1, 1); currentDate <= dateOfFinalAge; currentDate = currentDate.AddMonths(1))
        {
            decimal salary = InsuredSalary(currentDate.Year, person).IfLeft(decimal.Zero);

            TechnicalAge age = TechnicalAge.From(currentDate.Year, currentDate.Month) - birthdateAsAge;

            salaries.Add(new BvgTimeSeriesPoint(age, currentDate, salary));
        }

        return salaries.ToArray();
    }

    public Either<string, BvgTimeSeriesPoint[]> RetirementCreditFactors(int calculationYear, BvgPerson person)
    {
        DateTime technicalBirthdate = person.DateOfBirth.GetBirthdateTechnical();
        TechnicalAge birthdateAsAge = TechnicalAge.From(technicalBirthdate.Year, technicalBirthdate.Month);
        DateTime dateOfFinalAge = GetRetirementDate(person.DateOfBirth, person.Gender);

        List<BvgTimeSeriesPoint> points = [];
        for (DateTime currentDate = new DateTime(calculationYear, 1, 1); currentDate <= dateOfFinalAge; currentDate = currentDate.AddMonths(1))
        {
            int xBvg = currentDate.Year - person.DateOfBirth.Year;
            decimal factor = currentDate < StartOfBvgRevision
                ? retirementCredits.GetRate(xBvg)
                : RetirementCreditFactor(xBvg);

            TechnicalAge age = TechnicalAge.From(currentDate.Year, currentDate.Month) - birthdateAsAge;

            points.Add(new BvgTimeSeriesPoint(age, currentDate, factor));
        }

        return points.ToArray();
    }

    public Either<string, BvgTimeSeriesPoint[]> RetirementCredits(int calculationYear, BvgPerson person)
    {
        return from factors in RetirementCreditFactors(calculationYear, person)
            from salaries in InsuredSalaries(calculationYear, person)
               select Combine(factors, salaries).ToArray();

        IEnumerable<BvgTimeSeriesPoint> Combine(BvgTimeSeriesPoint[] factors, BvgTimeSeriesPoint[] salaries)
        {
            return from f in factors
                from s in salaries
                    where f.Date == s.Date
                    select f with {Value = f.Value * s.Value};
        }
    }

    private Either<string, BvgCalculationResult> CalculateInternal(int calculationYear, decimal retirementCapitalEndOfYear, BvgPerson person)
    {
        DateTime retirementDate = GetRetirementDate(person.DateOfBirth, person.Gender);

        decimal insuredSalary = InsuredSalary(calculationYear, person).IfLeft(() => decimal.Zero);

        decimal retirementCreditFactor = GetRetirementCreditFactor(person, calculationYear);

        BvgTimeSeriesPoint[] retirementCreditSequence = RetirementCredits(calculationYear, person).IfLeft(() => []);

        IReadOnlyCollection<RetirementCapital> retirementCapitalSequence =
            GetRetirementCapitalSequence(retirementCapitalEndOfYear, calculationYear, person, retirementCreditSequence);

        DateTime processingDate = new(calculationYear, 1, 1);

        decimal retirementCredit = retirementCreditSequence.SingleOrDefault(item => item.Date == processingDate) switch
        {
            null => decimal.Zero,
            { } item => item.Value
        };

        RetirementCapital finalRetirementCapital = retirementCapitalSequence.SingleOrDefault(item => item.Date == retirementDate);

        decimal actualRetirementCapitalEndOfYear = ActualRetirementCapitalEndOfYear(calculationYear, retirementCapitalSequence, retirementDate);

        decimal finalRetirementCapitalWithInterest = finalRetirementCapital switch
        {
            null => decimal.Zero,
            not null => finalRetirementCapital.Value
        };

        decimal finalRetirementCapitalWithoutInterest = finalRetirementCapital switch
        {
            null => decimal.Zero,
            not null => finalRetirementCapital.ValueWithoutInterest
        };

        decimal retirementPension = RetirementPension(finalRetirementCapitalWithInterest, retirementDate, CurrentBvgCalculatorFunc(retirementCredit));

        // reset risk benefits to 0 if below salary threshold
        decimal disabilityPension = DisabilityPension(finalRetirementCapitalWithoutInterest, retirementDate, CurrentBvgCalculatorFunc(retirementCredit));
        decimal partnerPension = disabilityPension * Bvg.FactorPartnersPension;
        decimal childPension = disabilityPension * Bvg.FactorChildPension;
        decimal orphanPension = childPension;

        Either<string, BvgCalculationResult> result = new BvgCalculationResult
        {
            DateOfRetirement = GetRetirementDate(person.DateOfBirth, person.Gender),
            EffectiveSalary = person.ReportedSalary,
            InsuredSalary = insuredSalary,
            RetirementCredit = retirementCredit,
            RetirementCreditFactor = retirementCreditFactor,
            RetirementPension = retirementPension,
            RetirementCapitalEndOfYear = actualRetirementCapitalEndOfYear,
            FinalRetirementCapital = finalRetirementCapitalWithInterest,
            FinalRetirementCapitalWithoutInterest = finalRetirementCapitalWithoutInterest,
            DisabilityPension = disabilityPension,
            PartnerPension = partnerPension,
            OrphanPension = orphanPension,
            ChildPensionForDisabled = childPension,
            RetirementCreditSequence = retirementCreditSequence.Select(item => new RetirementCredit(item.Value, item.Age.Years)).ToList(),
            RetirementCapitalSequence = retirementCapitalSequence
        };

        return result;

        Func<BvgCalculationResult> CurrentBvgCalculatorFunc(decimal currentRetirementCapitalEndOfYear)
        {
            return () => bvgCalculator.Calculate(calculationYear, currentRetirementCapitalEndOfYear, person).IfLeft(() => null);
        }
    }

    private static decimal ActualRetirementCapitalEndOfYear(int calculationYear,
        IReadOnlyCollection<RetirementCapital> retirementCapitalSequence, DateTime retirementDate)
    {
        DateTime followingYearDate = new DateTime(calculationYear + 1, 1, 1);

        RetirementCapital match = retirementCapitalSequence.SingleOrDefault(item => item.Date == followingYearDate);

        if (match is null)
        {
            if (followingYearDate > retirementDate)
            {
                return retirementCapitalSequence
                    .Where(item => item.Date.Year == calculationYear)
                    .MaxBy(item => item.Date)
                    .Value;
            }
            return decimal.Zero;
        }

        return match.Value;
    }

    private static decimal DisabilityPension(decimal finalRetirementCapitalWithoutInterest, DateTime retirementDate, Func<BvgCalculationResult> currentBvgCalculatorFunc)
    {
        if (retirementDate < StartOfBvgRevision)
        {
            return currentBvgCalculatorFunc().DisabilityPension;
        }

        return finalRetirementCapitalWithoutInterest * PensionConversionRate;
    }

    public bool IsRetired(BvgPerson person, DateTime dateOfProcess)
    {
        DateTime retiredAt = GetRetirementDate(person.DateOfBirth, person.Gender);

        return retiredAt.AddDays(-1) < dateOfProcess.Date;
    }

    public DateTime GetRetirementDate(DateTime dateOfBirth, Gender gender)
    {
        return retirementDateCalculator.DateOfRetirement(gender, dateOfBirth);
    }

    public TechnicalAge GetRetirementAge(Gender typeOfGender, DateTime birthdate)
    {
        (int years, int months) = retirementDateCalculator.RetirementAge(typeOfGender, birthdate);

        return TechnicalAge.From(years, months);
    }

    private decimal RetirementPension(decimal finalRetirementCapital, DateTime retirementDate, Func<BvgCalculationResult> currentBvgCalculatorFunc)
    {
        if(retirementDate < StartOfBvgRevision)
        {
            return currentBvgCalculatorFunc().RetirementPension;
        }

        return finalRetirementCapital * PensionConversionRate;
    }

    private IReadOnlyCollection<RetirementCapital> GetRetirementCapitalSequence(
        decimal retirementCapitalEndOfYear,
        int calculationYear,
        BvgPerson personDetails,
        BvgTimeSeriesPoint[] retirementCreditSequence)
    {
        // Date of retirement
        DateTime dateOfRetirement = GetRetirementDate(personDetails.DateOfBirth, personDetails.Gender);

        // Interest rates
        decimal iBvg = Bvg.GetInterestRate(calculationYear);

        // Retirement assets at end of insurance period Bvg portion
        TechnicalAge retirementAgeBvg = GetRetirementAge(personDetails.Gender, personDetails.DateOfBirth);
        
        Func<TechnicalAge, decimal> retirementCreditGetter = age =>
        {
            return retirementCreditSequence.SingleOrDefault(p => p.Age == age)?.Value ?? decimal.Zero;
        };

        if (IsRetired(personDetails, new(calculationYear+1, 1, 1)))
        {
            return [new RetirementCapital(dateOfRetirement, retirementCapitalEndOfYear, retirementCapitalEndOfYear)];
        }

        return projectionCalculator.ProjectionTable(
            iBvg,
            dateOfRetirement,
            dateOfRetirement,
            retirementAgeBvg,
            retirementAgeBvg,
            calculationYear+1,
            retirementCapitalEndOfYear,
            retirementCreditGetter)
            .Select(item => new RetirementCapital(item.DateOfCalculation, item.RetirementCapital, item.RetirementCapitalWithoutInterest))
            .ToArray();
    }


    private decimal UnconditionedInsuredSalary(int calculationYear, decimal reportedSalary, decimal quota)
    {
        decimal ahvMax = Bvg.GetPensionMaximum(calculationYear);
        return Prelude.Some(MathUtils.Round(reportedSalary))

            // scale salary up
            .Map(salary => salary / quota)

            // check salary entrance level
            .Where(v => v > ahvMax * SalaryThresholdFactor)

            .Map(v => Math.Min(v, Bvg.GetMaximalInsurableSalary(ahvMax)))
            .Map(v => Math.Min(v, Bvg.GetMaximumSalary(ahvMax)))

            // reduce by coordination deduction
            .Map(v => v * (decimal.One - CoordinationDeductionFactor))

            // restrict by BVG salary max
            .Map(v => v * quota)
            .Map(MathUtils.Round5)
            .IfNone(decimal.Zero);
    }

    private decimal GetRetirementCreditFactor(BvgPerson person, int calculationYear)
    {
        int xBvg = calculationYear - person.DateOfBirth.Year;

        return retirementCredits.GetRate(xBvg);
    }

    private static decimal RetirementCreditFactor(int xBvg)
    {
        return xBvg switch
        {
            > 24 and <= 44 => 0.09M,
            > 44 and <= 65 => 0.14M,
            _ => 0
        };
    }

}
