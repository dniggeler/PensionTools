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
    IValidator<BvgPerson> bvgPersonValidator)
    : IBvgCalculator
{
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
        DateTime currentDate = dateOfProcess.BeginOfYear();
        DateTime retirementDate = retirementDateCalculator.DateOfRetirement(person.Gender, person.DateOfBirth);
        DateTime technicalBirthdate = person.DateOfBirth.GetBirthdateTechnical();
        TechnicalAge birthdateAsAge = TechnicalAge.From(technicalBirthdate.Year, technicalBirthdate.Month);

        List<BvgTimeSeriesPoint> salaries = [];
        while (currentDate < retirementDate)
        {
            decimal salary = InsuredSalary(currentDate, person).IfLeft(decimal.Zero);

            TechnicalAge age = TechnicalAge.From(currentDate.Year, currentDate.Month) - birthdateAsAge;

            salaries.Add(new BvgTimeSeriesPoint(age, currentDate, salary));
            currentDate = currentDate.AddYears(1);
        }

        return salaries.ToArray();
    }

    public Either<string, BvgTimeSeriesPoint[]> RetirementCreditFactors(DateTime dateOfProcess, BvgPerson person)
    {
        DateTime technicalBirthdate = person.DateOfBirth.GetBirthdateTechnical();
        TechnicalAge birthdateAsAge = TechnicalAge.From(technicalBirthdate.Year, technicalBirthdate.Month);

        List<BvgTimeSeriesPoint> points = [];
        foreach (var xBvg in Enumerable.Range(Bvg.EntryAgeBvg, Bvg.FinalAge - Bvg.EntryAgeBvg + 1))
        {
            DateTime calculationDate = new DateTime(person.DateOfBirth.Year + xBvg, 1, 1);
            decimal factor = calculationDate < StartOfBvgRevision
                ? retirementCredits.GetRate(xBvg)
                : RetirementCreditFactor(xBvg);

            TechnicalAge age = TechnicalAge.From(calculationDate.Year, calculationDate.Month) - birthdateAsAge;

            points.Add(new BvgTimeSeriesPoint(age, calculationDate, factor));
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

        IReadOnlyCollection<RetirementCredit> retirementCreditSequence =
            GetRetirementCreditSequence(person, dateOfProcess, salary);

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
            RetirementCreditSequence = retirementCreditSequence,
            RetirementCapitalSequence = retirementCapitalSequence
        };

        return result;
    }

    public bool IsRetired(BvgPerson person, DateTime dateOfProcess)
    {
        DateTime retiredAt = GetRetirementDate(person.DateOfBirth, person.Gender);

        return (new DateTime(retiredAt.Year, retiredAt.Month, 1).AddDays(-1) < dateOfProcess.Date);
    }

    public DateTime GetRetirementDate(DateTime dateOfBirth, Gender gender)
    {
        return retirementDateCalculator.DateOfRetirement(gender, dateOfBirth);
    }

    public (int Years, int Months) GetRetirementAge(Gender typeOfGender, DateTime birthdate)
    {
        return retirementDateCalculator.RetirementAge(typeOfGender, birthdate);
    }

    private Either<string, decimal> InsuredSalaryInternal(DateTime dateOfProcess, BvgPerson person)
    {
        return dateOfProcess < StartOfBvgRevision
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
        IReadOnlyCollection<RetirementCredit> retirementCreditSequence)
    {
        IEnumerable<RetirementCapital> retirementCapitalSequence =
            GetRetirementCapitalSequence(predecessorCapital, dateOfProcess, personDetails, retirementCreditSequence);

        RetirementCapital latestElement = retirementCapitalSequence.First();

        return MathUtils.Round((latestElement.Value) * Bvg.GetUwsRateBvg(dateOfProcess.Year, personDetails.Gender));
    }

    private decimal GetPartnerPension(
        IReadOnlyCollection<RetirementCapital> retirementCapitalSequence,
        DateTime dateOfProcess,
        BvgPerson personDetails)
    {
        decimal capital = GetFinalRetirementCapitalWithoutInterest(retirementCapitalSequence);

        return MathUtils.Round(capital * Bvg.FactorPartnersPension *
                               Bvg.GetUwsRateBvg(dateOfProcess.Year, personDetails.Gender));
    }

    private decimal GetChildPensionForDisabled(
        IReadOnlyCollection<RetirementCapital> retirementCapitalSequence,
        BvgPerson personDetails,
        DateTime dateOfProcess)
    {
        decimal capital = GetFinalRetirementCapitalWithoutInterest(retirementCapitalSequence);

        return MathUtils.Round(capital * Bvg.FactorChildPension *
                               Bvg.GetUwsRateBvg(dateOfProcess.Year, personDetails.Gender));
    }

    private decimal GetDisabilityPension(
        IReadOnlyCollection<RetirementCapital> retirementCapitalSequence, BvgPerson personDetails, DateTime dateOfProcess)
    {
        decimal capital = GetFinalRetirementCapitalWithoutInterest(retirementCapitalSequence);

        return MathUtils.Round(capital * Bvg.GetUwsRateBvg(dateOfProcess.Year, personDetails.Gender));
    }

    private decimal GetFinalRetirementCapital(
        IReadOnlyCollection<RetirementCapital> retirementCapitalSequence)
    {
        RetirementCapital final = retirementCapitalSequence.First();

        return MathUtils.Round(final.Value);
    }

    private decimal GetFinalRetirementCapitalWithoutInterest(IReadOnlyCollection<RetirementCapital> retirementCapitalSequence)
    {
        RetirementCapital final = retirementCapitalSequence.First();

        return MathUtils.Round(final.ValueWithoutInterest);
    }

    private IReadOnlyCollection<RetirementCapital> GetRetirementCapitalSequence(
        PredecessorRetirementCapital predecessorCapital,
        DateTime dateOfProcess,
        BvgPerson personDetails,
        IReadOnlyCollection<RetirementCredit> retirementCreditSequence)
    {
        // Date of retirement
        DateTime dateOfRetirement = GetRetirementDate(personDetails.DateOfBirth, personDetails.Gender);

        // Interest rates
        decimal iBvg = Bvg.GetInterestRate(dateOfProcess.Year);

        // Retirement assets at end of insurance period Bvg portion
        int age = dateOfProcess.Year - personDetails.DateOfBirth.Year;
        int retirementAgeBvg = GetRetirementAge(personDetails.Gender, personDetails.DateOfBirth).Years;

        return BvgCapitalCalculationHelper.GetRetirementCapitalSequence(dateOfProcess,
            dateOfRetirement,
            age,
            retirementAgeBvg,
            iBvg,
            predecessorCapital,
            retirementCreditSequence);
    }

    private IReadOnlyCollection<RetirementCredit> GetRetirementCreditSequence(
        BvgPerson personDetails,
        DateTime dateOfProcess,
        BvgSalary salaryDetails)
    {
        int xsBvg = GetRetirementAge(personDetails.Gender, personDetails.DateOfBirth).Years;
        int xBvg = personDetails.DateOfBirth.GetBvgAge(dateOfProcess.Year);

        BvgRetirementCreditsTable bvgRetirementCreditTable = new BvgRetirementCreditsTable();

        return Enumerable.Range(xBvg, xsBvg - xBvg + 1)
            .Select(x =>
                new RetirementCredit(bvgRetirementCreditTable.GetRate(x) * salaryDetails.InsuredSalary, x))
            .ToList();
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

        return MathUtils.Round5(insuredSalary.IfNone(0M));


        Option<decimal> GetInsuredSalaryWhenNotDisabled()
        {
            return Prelude.Some(person.ReportedSalary)

                // check salary entrance level
                .Where(v => v > maxAhvPension * SalaryThresholdFactor)

                // restrict by BVG salary max
                .Map(v => Math.Min(v, Bvg.GetMaximumSalary(maxAhvPension)))

                // reduce by coordination deduction
                .Map(v => v * (decimal.One - CoordinationDeductionFactor))

                .Map(v => Math.Round(v, 1, MidpointRounding.AwayFromZero));
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
                .Map(v => v < minSalary ? minSalary : v)
                .Map(MathUtils.Round5);
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
