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
    
    public Task<Either<string, BvgCalculationResult>> CalculateAsync(
        PredecessorRetirementCapital predecessorCapital, DateTime dateOfProcess, BvgPerson person)
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
            .Bind(_ => CalculateInternal(predecessorCapital, dateOfProcess, person))
            .AsTask();
    }

    public Either<string, decimal> InsuredSalary(DateTime dateOfProcess, BvgPerson person)
    {
        return InsuredSalaryInternal(dateOfProcess, person);
    }

    public Either<string, BvgTimeSeriesPoint[]> InsuredSalaries(DateTime dateOfProcess, BvgPerson person)
    {
        DateTime technicalBirthdate = person.DateOfBirth.GetBirthdateTechnical();
        TechnicalAge birthdateAsAge = (technicalBirthdate.Year, technicalBirthdate.Month);
        DateTime dateOfFinalAge = GetRetirementDate(person.DateOfBirth, person.Gender);

        List<BvgTimeSeriesPoint> salaries = [];
        for (DateTime currentDate = dateOfProcess; currentDate <= dateOfFinalAge; currentDate = currentDate.AddMonths(1))
        {
            decimal salary = InsuredSalary(currentDate, person).IfLeft(decimal.Zero);

            TechnicalAge age = TechnicalAge.From(currentDate.Year, currentDate.Month) - birthdateAsAge;

            salaries.Add(new BvgTimeSeriesPoint(age, currentDate, salary));
        }

        return salaries.ToArray();
    }

    public Either<string, BvgTimeSeriesPoint[]> RetirementCreditFactors(DateTime dateOfProcess, BvgPerson person)
    {
        DateTime technicalBirthdate = person.DateOfBirth.GetBirthdateTechnical();
        TechnicalAge birthdateAsAge = TechnicalAge.From(technicalBirthdate.Year, technicalBirthdate.Month);
        DateTime dateOfFinalAge = GetRetirementDate(person.DateOfBirth, person.Gender);

        List<BvgTimeSeriesPoint> points = [];
        for (DateTime currentDate = dateOfProcess; currentDate <= dateOfFinalAge; currentDate = currentDate.AddMonths(1))
        {
            int xBvg = currentDate.Year - person.DateOfBirth.Year;
            decimal factor = currentDate <= StartOfBvgRevision
                ? retirementCredits.GetRate(xBvg)
                : RetirementCreditFactor(xBvg);

            TechnicalAge age = TechnicalAge.From(currentDate.Year, currentDate.Month) - birthdateAsAge;

            points.Add(new BvgTimeSeriesPoint(age, currentDate, factor));
        }

        return points.ToArray();
    }

    public Either<string, BvgTimeSeriesPoint[]> RetirementCredits(DateTime dateOfProcess, BvgPerson person)
    {
        return from factors in RetirementCreditFactors(dateOfProcess, person)
            from salaries in InsuredSalaries(dateOfProcess, person)
               select Combine(factors, salaries).ToArray();

        IEnumerable<BvgTimeSeriesPoint> Combine(BvgTimeSeriesPoint[] factors, BvgTimeSeriesPoint[] salaries)
        {
            return from f in factors
                from s in salaries
                    where f.Date == s.Date
                    select f with {Value = f.Value * s.Value};
        }
    }

    private Either<string, BvgCalculationResult> CalculateInternal(
        PredecessorRetirementCapital predecessorCapital, in DateTime dateOfProcess, BvgPerson person)
    {
        BvgSalary salary = GetBvgSalary(dateOfProcess, person);

        decimal retirementCreditFactor = GetRetirementCreditFactor(person, dateOfProcess);
        decimal retirementCredit = salary.InsuredSalary * retirementCreditFactor;

        BvgTimeSeriesPoint[] retirementCreditSequence = RetirementCredits(dateOfProcess, person).IfLeft(() => []);

        IReadOnlyCollection<RetirementCapital> retirementCapitalSequence =
            GetRetirementCapitalSequence(predecessorCapital, dateOfProcess, person, retirementCreditSequence);

        decimal retirementCapitalEndOfYear =
            GetRetirementCapitalEndOfYear(dateOfProcess, retirementCapitalSequence);

        decimal finalRetirementCapital =
            GetFinalRetirementCapital(retirementCapitalSequence);

        decimal finalRetirementCapitalWithoutInterest =
            GetFinalRetirementCapitalWithoutInterest(retirementCapitalSequence);

        decimal retirementPension = GetRetirementPension(predecessorCapital, person, dateOfProcess, retirementCreditSequence);

        // reset risk benefits to 0 if below salary threshold
        decimal disabilityPension = 0;
        decimal partnerPension = 0;
        decimal childPension = 0;
        decimal orphanPension = 0;

        int year = dateOfProcess.Year;

        if (salary.EffectiveSalary > Bvg.GetEntranceThreshold(Bvg.GetPensionMaximum(year)))
        {
            disabilityPension = GetDisabilityPension(retirementCapitalSequence, person, dateOfProcess);
            partnerPension = GetPartnerPension(retirementCapitalSequence, dateOfProcess, person);
            childPension = GetChildPensionForDisabled(retirementCapitalSequence, person, dateOfProcess);
            orphanPension = childPension;
        }

        Either<string, BvgCalculationResult> result = new BvgCalculationResult
        {
            DateOfRetirement = GetRetirementDate(person.DateOfBirth, person.Gender),
            EffectiveSalary = salary.EffectiveSalary,
            InsuredSalary = salary.InsuredSalary,
            RetirementCredit = retirementCredit,
            RetirementCreditFactor = retirementCreditFactor,
            RetirementPension = retirementPension,
            RetirementCapitalEndOfYear = retirementCapitalEndOfYear,
            FinalRetirementCapital = finalRetirementCapital,
            FinalRetirementCapitalWithoutInterest = finalRetirementCapitalWithoutInterest,
            DisabilityPension = disabilityPension,
            PartnerPension = partnerPension,
            OrphanPension = orphanPension,
            ChildPensionForDisabled = childPension,
            RetirementCreditSequence = retirementCreditSequence.Select(item => new RetirementCredit(item.Value, item.Age.Years)).ToList(),
            RetirementCapitalSequence = retirementCapitalSequence
        };

        return result;
    }

    public bool IsRetired(BvgPerson person, DateTime dateOfProcess)
    {
        DateTime retiredAt = GetRetirementDate(person.DateOfBirth, person.Gender);

        return retiredAt < dateOfProcess.Date;
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

    private Either<string, decimal> InsuredSalaryInternal(DateTime dateOfProcess, BvgPerson person)
    {
        return dateOfProcess <= StartOfBvgRevision
            ? bvgCalculator.InsuredSalary(dateOfProcess, person)
            : GetBvgSalary(dateOfProcess, person).InsuredSalary;
    }

    private BvgSalary GetBvgSalary(DateTime dateOfProcess, BvgPerson person)
    {
        decimal workingAbilityDegree = decimal.One - person.DisabilityDegree;

        BvgSalary salary = new BvgSalary();

        bool isRetired = IsRetired(person, dateOfProcess);

        if (isRetired && person.DisabilityDegree == decimal.One)
        {
            return salary;
        }

        salary.ReportedSalary = person.ReportedSalary;
        salary.EffectiveSalary = person.ReportedSalary * workingAbilityDegree;
        salary.InsuredSalary = isRetired ? 0M : GetInsuredSalary(person, dateOfProcess);

        return salary;
    }

    private decimal GetRetirementCapitalEndOfYear(DateTime dateOfProcess,
        IReadOnlyCollection<RetirementCapital> retirementCapitalSequence)
    {
        DateTime endOfYearDate = dateOfProcess.GetEndOfYearDate();

        return retirementCapitalSequence
            .Where(item => item.Date == endOfYearDate.Date)
            .Select(item => item.Value)
            .DefaultIfEmpty(retirementCapitalSequence.Last().Value)
            .Single();
    }

    private decimal GetRetirementPension(
        PredecessorRetirementCapital predecessorCapital,
        BvgPerson personDetails,
        DateTime dateOfProcess,
        BvgTimeSeriesPoint[] retirementCreditSequence)
    {
        IEnumerable<RetirementCapital> retirementCapitalSequence =
            GetRetirementCapitalSequence(predecessorCapital, dateOfProcess, personDetails, retirementCreditSequence);

        RetirementCapital latestElement = retirementCapitalSequence.MaxBy(item => item.Date);

        if (latestElement.Date < StartOfBvgRevision)
        {
            return latestElement.Value * Bvg.GetUwsRateBvg(dateOfProcess.Year, personDetails.Gender);
        }

        return latestElement.Value * PensionConversionRate;
    }

    private decimal GetPartnerPension(
        IReadOnlyCollection<RetirementCapital> retirementCapitalSequence,
        DateTime dateOfProcess,
        BvgPerson personDetails)
    {
        decimal capital = GetFinalRetirementCapitalWithoutInterest(retirementCapitalSequence);

        return capital * Bvg.FactorPartnersPension * Bvg.GetUwsRateBvg(dateOfProcess.Year, personDetails.Gender);
    }

    private decimal GetChildPensionForDisabled(
        IReadOnlyCollection<RetirementCapital> retirementCapitalSequence,
        BvgPerson personDetails,
        DateTime dateOfProcess)
    {
        decimal capital = GetFinalRetirementCapitalWithoutInterest(retirementCapitalSequence);

        return capital * Bvg.FactorChildPension * Bvg.GetUwsRateBvg(dateOfProcess.Year, personDetails.Gender);
    }

    private decimal GetDisabilityPension(
        IReadOnlyCollection<RetirementCapital> retirementCapitalSequence, BvgPerson personDetails, DateTime dateOfProcess)
    {
        decimal capital = GetFinalRetirementCapitalWithoutInterest(retirementCapitalSequence);

        return capital * Bvg.GetUwsRateBvg(dateOfProcess.Year, personDetails.Gender);
    }

    private decimal GetFinalRetirementCapital(
        IReadOnlyCollection<RetirementCapital> retirementCapitalSequence)
    {
        return retirementCapitalSequence.MaxBy(item => item.Date).Value;
    }

    private decimal GetFinalRetirementCapitalWithoutInterest(IReadOnlyCollection<RetirementCapital> retirementCapitalSequence)
    {
        return retirementCapitalSequence.MaxBy(item => item.Date).ValueWithoutInterest;
    }

    private IReadOnlyCollection<RetirementCapital> GetRetirementCapitalSequence(
        PredecessorRetirementCapital predecessorCapital,
        DateTime dateOfProcess,
        BvgPerson personDetails,
        BvgTimeSeriesPoint[] retirementCreditSequence)
    {
        // Date of retirement
        DateTime dateOfRetirement = GetRetirementDate(personDetails.DateOfBirth, personDetails.Gender);

        // Interest rates
        decimal iBvg = Bvg.GetInterestRate(dateOfProcess.Year);

        // Retirement assets at end of insurance period Bvg portion
        TechnicalAge retirementAgeBvg = GetRetirementAge(personDetails.Gender, personDetails.DateOfBirth);


        Func<TechnicalAge, decimal> retirementCreditGetter = age =>
        {
            return retirementCreditSequence.SingleOrDefault(p => p.Age == age)?.Value ?? decimal.Zero;
        };

        return projectionCalculator.ProjectionTable(
            iBvg,
            dateOfRetirement,
            dateOfRetirement,
            retirementAgeBvg,
            retirementAgeBvg,
            dateOfProcess.Year,
            predecessorCapital.EndOfYearAmount,
            retirementCreditGetter)
            .Select(item => new RetirementCapital(item.DateOfCalculation, item.RetirementCapital, item.RetirementCapitalWithoutInterest))
            .ToArray();
    }

    private decimal GetRetirementCreditFactor(BvgPerson person, DateTime dateOfProcess)
    {
        int xBvg = dateOfProcess.Year - person.DateOfBirth.Year;

        return retirementCredits.GetRate(xBvg);
    }

    private static decimal GetInsuredSalary(BvgPerson person, DateTime dateOfProcess)
    {
        const decimal fullEmployedDegree = 1.0M;

        int year = dateOfProcess.Year;
        decimal maxAhvPension = Bvg.GetPensionMaximum(year);

        Option<decimal> insuredSalary = 0M;

        if (person.DisabilityDegree == fullEmployedDegree)
        {
            return insuredSalary.IfNone(0M);
        }

        if (person.DisabilityDegree > 0M)
        {
            insuredSalary = GetInsuredSalaryWhenDisabled();
        }
        else
        {
            insuredSalary = GetInsuredSalaryWhenNotDisabled();
        }

        return insuredSalary.IfNone(0M);


        Option<decimal> GetInsuredSalaryWhenNotDisabled()
        {
            return Prelude.Some(person.ReportedSalary)

                // check salary entrance level
                .Where(v => v > maxAhvPension * SalaryThresholdFactor)

                // restrict by BVG salary max
                .Map(v => Math.Min(v, Bvg.GetMaximumSalary(maxAhvPension)))

                // reduce by coordination deduction
                .Map(v => v * (decimal.One - CoordinationDeductionFactor));
        }

        Option<decimal> GetInsuredSalaryWhenDisabled()
        {
            decimal minSalary = Bvg.GetMinimumSalary(maxAhvPension);

            Option<decimal> disabilityDegree = person.DisabilityDegree;

            return disabilityDegree
                .Where(v => v is > 0 and < decimal.One)
                .Map(v => fullEmployedDegree - v)

                // scale salary up
                .Map(v => person.ReportedSalary / v)

                // check salary entrance level
                .Where(v => v > Bvg.GetEntranceThreshold(maxAhvPension))

                .Map(v => Math.Min(v, Bvg.GetMaximumSalary(maxAhvPension)))

                // reduce by coordination deduction
                .Map(v => v * (decimal.One - CoordinationDeductionFactor))

                // restrict by BVG salary max
                .Map(v => v * (fullEmployedDegree - person.DisabilityDegree))
                .Map(v => v < minSalary ? minSalary : v);
        }
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
