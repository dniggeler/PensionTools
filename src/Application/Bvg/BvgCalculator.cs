using Application.Bvg.Models;
using Application.Extensions;
using Domain.Enums;
using Domain.Models.Bvg;
using FluentValidation;
using FluentValidation.Results;
using LanguageExt;
using PensionCoach.Tools.CommonUtils;

namespace Application.Bvg;

public class BvgCalculator(
    BvgRetirementDateCalculator retirementDateCalculator,
    IBvgRetirementCredits retirementCredits,
    IValidator<BvgPerson> bvgPersonValidator)
    : IBvgCalculator
{
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
        BvgSalary salary = GetBvgSalary(calculationYear, person);

        return salary.InsuredSalary;
    }

    public Either<string, BvgTimeSeriesPoint[]> InsuredSalaries(int calculationYear, BvgPerson person)
    {
        DateTime currentDate = new(calculationYear, 1, 1);
        DateTime retirementDate = retirementDateCalculator.DateOfRetirement(person.Gender, person.DateOfBirth);
        DateTime technicalBirthdate = person.DateOfBirth.GetBirthdateTechnical();
        TechnicalAge birthdateAsAge = TechnicalAge.From(technicalBirthdate.Year, technicalBirthdate.Month);

        List<BvgTimeSeriesPoint> salaries = [];
        decimal salary = InsuredSalary(calculationYear, person).IfLeft(decimal.Zero);
        while (currentDate <= retirementDate)
        {
            TechnicalAge age = TechnicalAge.From(currentDate.Year, currentDate.Month) - birthdateAsAge;
            salaries.Add(new BvgTimeSeriesPoint(age, currentDate, salary));
            currentDate = currentDate.AddYears(1);
        }

        return salaries.ToArray();
    }

    public Either<string, BvgTimeSeriesPoint[]> RetirementCreditFactors(int calculationYear, BvgPerson person)
    {
        DateTime technicalBirthdate = person.DateOfBirth.GetBirthdateTechnical();
        TechnicalAge birthdateAsAge = TechnicalAge.From(technicalBirthdate.Year, technicalBirthdate.Month);

        List<BvgTimeSeriesPoint> points = [];
        foreach (var xBvg in Enumerable.Range(Bvg.EntryAgeBvg, Bvg.FinalAge - Bvg.EntryAgeBvg + 1))
        {
            DateTime calculationDate = new DateTime(person.DateOfBirth.Year + xBvg, 1, 1);
            TechnicalAge age = TechnicalAge.From(calculationDate.Year, calculationDate.Month) - birthdateAsAge;
            points.Add(new BvgTimeSeriesPoint(age, calculationDate, retirementCredits.GetRate(xBvg)));
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
                select f with { Value = f.Value * s.Value };
        }
    }

    private Either<string, BvgCalculationResult> CalculateInternal(int calculationYear, decimal retirementCapitalEndOfYear, BvgPerson person)
    {
        BvgSalary salary = GetBvgSalary(calculationYear, person);

        decimal retirementCreditFactor = GetRetirementCreditFactor(person, calculationYear);
        decimal retirementCredit = salary.InsuredSalary * retirementCreditFactor;

        IReadOnlyCollection<RetirementCredit> retirementCreditSequence =
            GetRetirementCreditSequence(person, calculationYear, salary);

        IReadOnlyCollection<RetirementCapital> retirementCapitalSequence =
            GetRetirementCapitalSequence(retirementCapitalEndOfYear, calculationYear, person, retirementCreditSequence);

        decimal finalRetirementCapital =
            GetFinalRetirementCapital(retirementCapitalSequence);

        decimal finalRetirementCapitalWithoutInterest =
            GetFinalRetirementCapitalWithoutInterest(retirementCapitalSequence);

        DateTime dateOfProcess = new DateTime(calculationYear, 1, 1);

        decimal retirementPension = GetRetirementPension(retirementCapitalEndOfYear, person, dateOfProcess, retirementCreditSequence);

        // reset risk benefits to 0 if below salary threshold
        decimal disabilityPension = 0;
        decimal partnerPension = 0;
        decimal childPension = 0;
        decimal orphanPension = 0;

        if (salary.EffectiveSalary > Bvg.GetEntranceThreshold(Bvg.GetPensionMaximum(calculationYear)))
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

    private BvgSalary GetBvgSalary(int calculationYear, BvgPerson person)
    {
        decimal workingAbilityDegree = decimal.One - person.DisabilityDegree;

        BvgSalary salary = new BvgSalary();

        bool isRetired = IsRetired(person, new DateTime(calculationYear));

        if (isRetired && person.DisabilityDegree == decimal.One)
        {
            return salary;
        }

        salary.ReportedSalary = person.ReportedSalary;
        salary.EffectiveSalary = person.ReportedSalary * workingAbilityDegree;
        salary.InsuredSalary = isRetired ? 0M : GetInsuredSalary(person, calculationYear);

        return salary;
    }

    private decimal GetRetirementPension(
        decimal retirementEndOfYear,
        BvgPerson personDetails,
        DateTime dateOfProcess,
        IReadOnlyCollection<RetirementCredit> retirementCreditSequence)
    {
        IEnumerable<RetirementCapital> retirementCapitalSequence =
            GetRetirementCapitalSequence(retirementEndOfYear, dateOfProcess.Year, personDetails, retirementCreditSequence);

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
        decimal retirementEndOfYear,
        int calculationYear,
        BvgPerson personDetails,
        IReadOnlyCollection<RetirementCredit> retirementCreditSequence)
    {
        // Date of retirement
        DateTime dateOfRetirement = GetRetirementDate(personDetails.DateOfBirth, personDetails.Gender);

        // Interest rates
        decimal iBvg = Bvg.GetInterestRate(calculationYear);

        // Retirement assets at end of insurance period Bvg portion
        int age = calculationYear - personDetails.DateOfBirth.Year;
        var retirementAgeBvg = GetRetirementAge(personDetails.Gender, personDetails.DateOfBirth);

        return BvgCapitalCalculationHelper.GetRetirementCapitalSequence(calculationYear,
            dateOfRetirement,
            age,
            retirementAgeBvg.Years,
            iBvg,
            retirementEndOfYear,
            retirementCreditSequence);
    }

    private IReadOnlyCollection<RetirementCredit> GetRetirementCreditSequence(
        BvgPerson personDetails,
        int calculationYear,
        BvgSalary salaryDetails)
    {
        int xsBvg = GetRetirementAge(personDetails.Gender, personDetails.DateOfBirth).Years;
        int xBvg = personDetails.DateOfBirth.GetBvgAge(calculationYear);

        BvgRetirementCreditsTable bvgRetirementCreditTable = new BvgRetirementCreditsTable();

        return Enumerable.Range(xBvg, xsBvg - xBvg + 1)
            .Select(x =>
                new RetirementCredit(bvgRetirementCreditTable.GetRate(x) * salaryDetails.InsuredSalary, x))
            .ToList();
    }

    private decimal GetRetirementCreditFactor(BvgPerson person, int calculationYear)
    {
        int xBvg = calculationYear - person.DateOfBirth.Year;

        return retirementCredits.GetRate(xBvg);
    }

    private static decimal GetInsuredSalary(BvgPerson person, int calculationYear)
    {
        const decimal fullEmployedDegree = 1.0M;

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
            decimal ahvMax = Bvg.GetPensionMaximum(calculationYear);

            return Prelude.Some(person.ReportedSalary)

                // check salary entrance level
                .Where(v => v > Bvg.GetEntranceThreshold(ahvMax))

                // restrict by BVG salary max
                .Map(v => Math.Min(v, Bvg.GetMaximumSalary(ahvMax)))

                // reduce by coordination deduction
                .Map(v => v - GetCoordinationDeduction())

                .Map(v => Math.Max(v, Bvg.GetMinimumSalary(ahvMax)))
                .Map(v => Math.Round(v, 0, MidpointRounding.AwayFromZero));
        }

        Option<decimal> GetInsuredSalaryWhenDisabled()
        {
            decimal minSalary = Bvg.GetMinimumSalary(Bvg.GetPensionMaximum(calculationYear));

            Option<decimal> disabilityDegree = person.DisabilityDegree;

            return disabilityDegree
                .Where(v => v is > 0 and < decimal.One)
                .Map(v => fullEmployedDegree - v)

                // scale salary up
                .Map(v => person.ReportedSalary / v)

                // check salary entrance level
                .Where(v => v > Bvg.GetEntranceThreshold(Bvg.GetPensionMaximum(calculationYear)))

                .Map(v => Math.Min(v, Bvg.GetMaximumSalary(Bvg.GetPensionMaximum(calculationYear))))

                // reduce by coordination deduction
                .Map(v => v - GetCoordinationDeduction())

                // restrict by BVG salary max
                .Map(v => v * (fullEmployedDegree - person.DisabilityDegree))
                .Map(v => v < minSalary ? minSalary : v)
                .Map(MathUtils.Round5);
        }

        decimal GetCoordinationDeduction()
        {
            return Bvg.GetCoordinationDeduction(Bvg.GetPensionMaximum(calculationYear));
        }
    }
}
