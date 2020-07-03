using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.BvgCalculator.Models;

namespace PensionCoach.Tools.BvgCalculator
{
    public class BvgCalculator : IBvgCalculator
    {
        public Task<BvgCalculationResult> CalculateAsync(DateTime dateOfProcess, BvgPerson person)
        {
            BvgSalary salary = GetBvgSalary(dateOfProcess, person);

            decimal retirementCreditFactor = GetRetirementCreditFactor(person, dateOfProcess);
            decimal retirementCredit = GetRetirementCredit(person, salary.InsuredSalary, retirementCreditFactor);

            IReadOnlyCollection<RetirementCredit> retirementCreditSequence =
                GetRetirementCreditSequence(person, dateOfProcess, salary, plan);

            IReadOnlyCollection<BvgRetirementCapitalElement> retirementCapitalSequence =
                GetRetirementCapitalSequence(predecessorProcess, dateOfProcess, person, retirementCreditSequence, plan);

            decimal retirementCapitalEndOfYear =
                GetRetirementCapitalEndOfYear(dateOfProcess, retirementCapitalSequence);

            decimal finalRetirementCapital =
                GetFinalRetirementCapital(retirementCapitalSequence);

            decimal finalRetirementCapitalWithoutInterest =
                GetFinalRetirementCapitalWithoutInterest(retirementCapitalSequence);

            decimal retirementPension = GetRetirementPension(predecessorProcess, person, dateOfProcess, retirementCreditSequence, plan);

            // reset risk benefits to 0 if below salary threshold
            decimal disabilityPension = 0;
            decimal partnerPension = 0;
            decimal childPension = 0;
            decimal orphanPension = 0;

            int year = dateOfProcess.Year;

            if (salary.EffectiveSalary > Bvg.GetEntranceThreshold(Ahv.GetPensionMaximum(year)))
            {
                disabilityPension = GetDisabilityPension(retirementCapitalSequence, person, dateOfProcess, plan);
                partnerPension = GetPartnerPension(retirementCapitalSequence, dateOfProcess, person, plan);
                childPension = GetChildPensionForDisabled(retirementCapitalSequence, person, dateOfProcess, plan);
                orphanPension = childPension;
            }

            BvgBenefitsCalculatorResult result = new BvgBenefitsCalculatorResult
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

        private static BvgSalary GetBvgSalary(DateTime dateOfProcess, BvgPerson person)
        {
            decimal workingAbilityDegree = decimal.One - person.DisabilityDegree;

            BvgSalary salary = new BvgSalary();

            bool isRetired = plan.IsRetired(person, dateOfProcess);

            if (isRetired && person.DisabilityDegree == decimal.One)
            {
                return BvgSalary;
            }

            BvgSalary.ReportedSalary = person.ReportedSalary;
            BvgSalary.EffectiveSalary = person.ReportedSalary * workingAbilityDegree;
            BvgSalary.InsuredSalary = (isRetired || plan.IsSeniorPlan) ? 0M : GetInsuredSalary(person, dateOfProcess);

            return BvgSalary;
        }

        private decimal GetRetirementCapitalEndOfYear(DateTime dateOfProcess,
            IReadOnlyCollection<BvgRetirementCapitalElement> retirementCapitalSequence)
        {
            DateTime endOfYearDate = dateOfProcess.GetEndOfYearDate();

            return retirementCapitalSequence
                .Where(item => item.Date == endOfYearDate.Date)
                .Select(item => item.Value)
                .DefaultIfEmpty(retirementCapitalSequence.Last().Value)
                .Single();
        }

        private decimal GetRetirementPension(
            BvgProcessActuarialReserve predecessorProcess,
            BvgPerson personDetails,
            DateTime dateOfProcess,
            IReadOnlyCollection<RetirementCredit> retirementCreditSequence,
            BvgPensionPlan plan)
        {
            IEnumerable<BvgRetirementCapitalElement> retirementCapitalSequence =
                GetRetirementCapitalSequence(predecessorProcess, dateOfProcess, personDetails, retirementCreditSequence, plan);

            BvgRetirementCapitalElement latestElement = retirementCapitalSequence.First();

            return DigisMath.Round((latestElement.Value) * Bvg.GetUwsRateBvg(dateOfProcess.Year, personDetails.Gender));
        }

        private decimal GetPartnerPension(
            IReadOnlyCollection<BvgRetirementCapitalElement> retirementCapitalSequence,
            DateTime dateOfProcess,
            BvgPerson personDetails,
            BvgPensionPlan plan)
        {
            if (plan.IsSeniorPlan)
            {
                return 0M;
            }

            if (personDetails.HasStop && personDetails.HasStopWithRiskProtection == false)
            {
                return 0M;
            }

            decimal capital = GetFinalRetirementCapitalWithoutInterest(retirementCapitalSequence);

            return DigisMath.Round(capital * Bvg.FactorPartnersPension *
                                   Bvg.GetUwsRateBvg(dateOfProcess.Year, personDetails.Gender));
        }

        private decimal GetChildPensionForDisabled(
            IReadOnlyCollection<BvgRetirementCapitalElement> retirementCapitalSequence,
            BvgPerson personDetails,
            DateTime dateOfProcess,
            BvgPensionPlan plan)
        {
            if (plan.IsSeniorPlan)
            {
                return 0M;
            }

            if (personDetails.HasStop && personDetails.HasStopWithRiskProtection == false)
            {
                return 0M;
            }

            decimal capital = GetFinalRetirementCapitalWithoutInterest(retirementCapitalSequence);

            return DigisMath.Round(capital * Bvg.FactorChildPension *
                                   Bvg.GetUwsRateBvg(dateOfProcess.Year, personDetails.Gender));
        }

        private decimal GetDisabilityPension(IReadOnlyCollection<BvgRetirementCapitalElement> retirementCapitalSequence,
            BvgPerson personDetails, DateTime dateOfProcess, BvgPensionPlan plan)
        {
            if (plan.IsSeniorPlan)
            {
                return 0M;
            }

            if (personDetails.HasStop && personDetails.HasStopWithRiskProtection == false)
            {
                return 0M;
            }

            decimal capital = GetFinalRetirementCapitalWithoutInterest(retirementCapitalSequence);

            return DigisMath.Round(capital * Bvg.GetUwsRateBvg(dateOfProcess.Year, personDetails.Gender));
        }

        private decimal GetFinalRetirementCapital(
            IReadOnlyCollection<BvgRetirementCapitalElement> retirementCapitalSequence)
        {
            BvgRetirementCapitalElement final = retirementCapitalSequence.First();

            return DigisMath.Round(final.Value);
        }

        private decimal GetFinalRetirementCapitalWithoutInterest(IReadOnlyCollection<BvgRetirementCapitalElement> retirementCapitalSequence)
        {
            BvgRetirementCapitalElement final = retirementCapitalSequence.First();

            return DigisMath.Round(final.ValueWithoutInterest);
        }

        private IReadOnlyCollection<BvgRetirementCapitalElement> GetRetirementCapitalSequence(
            BvgProcessActuarialReserve predecessorProcess,
            DateTime dateOfProcess,
            BvgPerson personDetails,
            IReadOnlyCollection<RetirementCredit> retirementCreditSequence,
            BvgPensionPlan plan)
        {
            // Date of retirement
            DateTime dateOfRetirement = plan.GetRetirementDate(personDetails.DateOfBirth, personDetails.Gender);

            // Interest rates
            decimal iBvg = Bvg.GetInterestRate(dateOfProcess.Year);

            // Retirement assets at end of insurance period Bvg portion
            int age = dateOfProcess.Year - personDetails.DateOfBirth.Year;
            int retirementAgeBvg = plan.GetRetirementAge(personDetails.Gender);

            return BvgCapitalCalculationHelper.GetRetirementCapitalSequence(dateOfProcess,
                dateOfRetirement,
                age,
                retirementAgeBvg,
                iBvg,
                predecessorProcess,
                retirementCreditSequence);
        }

        private IReadOnlyCollection<RetirementCredit> GetRetirementCreditSequence(
            BvgPerson personDetails,
            DateTime dateOfProcess,
            BvgSalary BvgSalary,
            BvgPensionPlan plan)
        {
            return BvgCapitalCalculationHelper.GetRetirementCreditSequence(personDetails, dateOfProcess, BvgSalary, plan);
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

        private static decimal GetRetirementCreditFactor(BvgPerson person, DateTime dateOfProcess)
        {
            if (plan.IsSeniorPlan)
            {
                return decimal.Zero;
            }

            int xBvg = dateOfProcess.Year - person.DateOfBirth.Year;

            return CreditsTable.GetRate(xBvg);
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

            return DigisMath.Round5(insuredSalary.IfNone(0M));

            Option<decimal> GetInsuredSalaryWhenNotDisabled()
            {
                return toOption<decimal>(person.ReportedSalary)

                    // check salary entrance level
                    .Where(v => v > Bvg.GetEntranceThreshold(Ahv.GetPensionMaximum(year)))

                    // restrict by BVG salary max
                    .Map(v => Math.Min(v, Bvg.GetMaximumSalary(Ahv.GetPensionMaximum(dateOfProcess.Year))))

                    // reduce by coordination deduction
                    .Map(v => v - GetCoordinationDeduction())

                    .Map(v => Math.Max(v, Bvg.GetMinimumSalary(Ahv.GetPensionMaximum(year))))
                    .Map(v => Math.Round(v, 0, MidpointRounding.AwayFromZero));
            }

            Option<decimal> GetInsuredSalaryWhenDisabled()
            {
                decimal minSalary = Bvg.GetMinimumSalary(Ahv.GetPensionMaximum(year));

                Option<decimal> disabilityDegree = person.DisabilityDegree;

                return disabilityDegree
                    .Where(v => v > 0 && v < decimal.One)
                    .Map(v => fullEmployedDegree - v)

                    // scale salary up
                    .Map(v => person.ReportedSalary / v)

                    // check salary entrance level
                    .Where(v => v > Bvg.GetEntranceThreshold(Ahv.GetPensionMaximum(year)))

                    .Map(v => Math.Min(v, Bvg.GetMaximumSalary(Ahv.GetPensionMaximum(dateOfProcess.Year))))

                    // reduce by coordination deduction
                    .Map(v => v - GetCoordinationDeduction())

                    // restrict by BVG salary max
                    .Map(v => v * (fullEmployedDegree - person.DisabilityDegree))
                    .Map(v => v < minSalary ? minSalary : v)
                    .Map(DigisMath.Round5);
            }

            decimal GetCoordinationDeduction()
            {
                return Bvg.GetCoordinationDeduction(Ahv.GetPensionMaximum(year));
            }
        }
    }
}
