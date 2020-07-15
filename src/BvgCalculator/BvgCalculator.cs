using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.BvgCalculator.Models;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonUtils;

namespace PensionCoach.Tools.BvgCalculator
{
    public class BvgCalculator : IBvgCalculator
    {
        private readonly IBvgRetirementCredits RetirementCredits;

        public BvgCalculator(IBvgRetirementCredits retirementCredits)
        {
            RetirementCredits = retirementCredits;
        }

        public Task<BvgCalculationResult> CalculateAsync(
            PredecessorRetirementCapital predecessorCapital, DateTime dateOfProcess, BvgPerson person)
        {
            BvgSalary salary = GetBvgSalary(dateOfProcess, person);

            decimal retirementCreditFactor = GetRetirementCreditFactor(person, dateOfProcess);
            decimal retirementCredit = GetRetirementCredit(person, salary.InsuredSalary, retirementCreditFactor);

            IReadOnlyCollection<RetirementCredit> retirementCreditSequence =
                GetRetirementCreditSequence(person, dateOfProcess, salary);

            IReadOnlyCollection<BvgRetirementCapital> retirementCapitalSequence =
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

            BvgCalculationResult result = new BvgCalculationResult
            {
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

            return Task.FromResult(result);

        }

        public static bool IsRetired(BvgPerson person, DateTime dateOfProcess)
        {
            DateTime retiredAt = GetRetirementDate(person.DateOfBirth, person.Gender);

            return (new DateTime(retiredAt.Year, retiredAt.Month, 1).AddDays(-1) < dateOfProcess.Date);
        }

        public static DateTime GetRetirementDate(DateTime dateOfBirth, Gender gender)
        {
            // FinalAgeByPlan BVG
            int xsBvg = GetRetirementAge(gender);

            // Date of retirement
            return dateOfBirth.GetRetirementDate(xsBvg);
        }

        public static int GetRetirementAge(Gender typeOfGender)
        {
            return typeOfGender switch
            {
                Gender.Female => Bvg.RetirementAgeWomen,
                Gender.Male => Bvg.RetirementAgeMen,
                _ => throw new ArgumentException(nameof(typeOfGender))
            };
        }

        private static BvgSalary GetBvgSalary(DateTime dateOfProcess, BvgPerson person)
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
            IReadOnlyCollection<BvgRetirementCapital> retirementCapitalSequence)
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
            IEnumerable<BvgRetirementCapital> retirementCapitalSequence =
                GetRetirementCapitalSequence(predecessorCapital, dateOfProcess, personDetails, retirementCreditSequence);

            BvgRetirementCapital latestElement = retirementCapitalSequence.First();

            return MathUtils.Round((latestElement.Value) * Bvg.GetUwsRateBvg(dateOfProcess.Year, personDetails.Gender));
        }

        private decimal GetPartnerPension(
            IReadOnlyCollection<BvgRetirementCapital> retirementCapitalSequence,
            DateTime dateOfProcess,
            BvgPerson personDetails)
        {
            if (personDetails.HasStop && personDetails.HasStopWithRiskProtection == false)
            {
                return 0M;
            }

            decimal capital = GetFinalRetirementCapitalWithoutInterest(retirementCapitalSequence);

            return MathUtils.Round(capital * Bvg.FactorPartnersPension *
                                   Bvg.GetUwsRateBvg(dateOfProcess.Year, personDetails.Gender));
        }

        private decimal GetChildPensionForDisabled(
            IReadOnlyCollection<BvgRetirementCapital> retirementCapitalSequence,
            BvgPerson personDetails,
            DateTime dateOfProcess)
        {
            if (personDetails.HasStop && personDetails.HasStopWithRiskProtection == false)
            {
                return 0M;
            }

            decimal capital = GetFinalRetirementCapitalWithoutInterest(retirementCapitalSequence);

            return MathUtils.Round(capital * Bvg.FactorChildPension *
                                   Bvg.GetUwsRateBvg(dateOfProcess.Year, personDetails.Gender));
        }

        private decimal GetDisabilityPension(
            IReadOnlyCollection<BvgRetirementCapital> retirementCapitalSequence, BvgPerson personDetails, DateTime dateOfProcess)
        {
            if (personDetails.HasStop && personDetails.HasStopWithRiskProtection == false)
            {
                return 0M;
            }

            decimal capital = GetFinalRetirementCapitalWithoutInterest(retirementCapitalSequence);

            return MathUtils.Round(capital * Bvg.GetUwsRateBvg(dateOfProcess.Year, personDetails.Gender));
        }

        private decimal GetFinalRetirementCapital(
            IReadOnlyCollection<BvgRetirementCapital> retirementCapitalSequence)
        {
            BvgRetirementCapital final = retirementCapitalSequence.First();

            return MathUtils.Round(final.Value);
        }

        private decimal GetFinalRetirementCapitalWithoutInterest(IReadOnlyCollection<BvgRetirementCapital> retirementCapitalSequence)
        {
            BvgRetirementCapital final = retirementCapitalSequence.First();

            return MathUtils.Round(final.ValueWithoutInterest);
        }

        private IReadOnlyCollection<BvgRetirementCapital> GetRetirementCapitalSequence(
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
            int retirementAgeBvg = GetRetirementAge(personDetails.Gender);

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
            BvgSalary salary)
        {
            return BvgCapitalCalculationHelper.GetRetirementCreditSequence(personDetails, dateOfProcess, salary);
        }

        private decimal GetRetirementCredit(
            BvgPerson personDetails,
            decimal insuredSalary,
            decimal retirementCreditFactor)
        {
            if (personDetails.HasStop)
            {
                return 0M;
            }

            return insuredSalary * retirementCreditFactor;
        }

        private decimal GetRetirementCreditFactor(BvgPerson person, DateTime dateOfProcess)
        {
            int xBvg = dateOfProcess.Year - person.DateOfBirth.Year;

            return RetirementCredits.GetRate(xBvg);
        }

        private static decimal GetInsuredSalary(BvgPerson person, DateTime dateOfProcess)
        {
            const decimal fullEmployedDegree = 1.0M;

            int year = dateOfProcess.Year;

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
                decimal ahvMax = Bvg.GetPensionMaximum(year);

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
                decimal minSalary = Bvg.GetMinimumSalary(Bvg.GetPensionMaximum(year));

                Option<decimal> disabilityDegree = person.DisabilityDegree;

                return disabilityDegree
                    .Where(v => v > 0 && v < decimal.One)
                    .Map(v => fullEmployedDegree - v)

                    // scale salary up
                    .Map(v => person.ReportedSalary / v)

                    // check salary entrance level
                    .Where(v => v > Bvg.GetEntranceThreshold(Bvg.GetPensionMaximum(year)))

                    .Map(v => Math.Min(v, Bvg.GetMaximumSalary(Bvg.GetPensionMaximum(dateOfProcess.Year))))

                    // reduce by coordination deduction
                    .Map(v => v - GetCoordinationDeduction())

                    // restrict by BVG salary max
                    .Map(v => v * (fullEmployedDegree - person.DisabilityDegree))
                    .Map(v => v < minSalary ? minSalary : v)
                    .Map(MathUtils.Round5);
            }

            decimal GetCoordinationDeduction()
            {
                return Bvg.GetCoordinationDeduction(Bvg.GetPensionMaximum(year));
            }
        }
    }
}
