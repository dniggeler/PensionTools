using System;
using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using PensionVersusCapitalCalculator.Abstractions;

namespace PensionVersusCapitalCalculator
{
    public class PensionVersusCapitalCalculator : IPensionVersusCapitalCalculator
    {
        private readonly IIncomeTaxCalculator incomeTaxCalculator;
        private readonly Func<Canton, ICapitalBenefitTaxCalculator> capitalBenefitCalculatorFunc;

        public PensionVersusCapitalCalculator(
            IIncomeTaxCalculator incomeTaxCalculator,
            Func<Canton, ICapitalBenefitTaxCalculator> capitalBenefitCalculatorFunc)
        {
            this.incomeTaxCalculator = incomeTaxCalculator;
            this.capitalBenefitCalculatorFunc = capitalBenefitCalculatorFunc;
        }

        public async Task<Option<decimal>> CalculateAsync(
            int calculationYear, int municipalityId, Canton canton, TaxPerson incomeTaxPerson, CapitalBenefitTaxPerson capitalBenefitTaxPerson)
        {
            ICapitalBenefitTaxCalculator capitalBenefitCalculator = capitalBenefitCalculatorFunc(canton);

            Either<string, CapitalBenefitTaxResult> capitalBenefitTaxCalculationResult =
                await capitalBenefitCalculator.CalculateAsync(calculationYear, municipalityId, canton, capitalBenefitTaxPerson);

            Either<string, SingleTaxResult> incomeTaxCalculationResult =
                await incomeTaxCalculator.CalculateAsync(calculationYear, municipalityId, canton, incomeTaxPerson);

            var r = from b in capitalBenefitTaxCalculationResult
                from i in incomeTaxCalculationResult
                select CalculateBreakEvenFactor(b,i);

            return r.IfLeft(()=>Option<decimal>.None);

            Option<decimal> CalculateBreakEvenFactor(CapitalBenefitTaxResult benefitTaxResult, SingleTaxResult incomeTaxResult)
            {
                decimal capitalBenefitNet = capitalBenefitTaxPerson.TaxableBenefits - benefitTaxResult.TotalTaxAmount;
                decimal incomeNet = incomeTaxPerson.TaxableIncome - incomeTaxResult.TotalTaxAmount;

                if (incomeNet == decimal.Zero)
                {
                    return Option<decimal>.None;
                }

                return capitalBenefitNet / incomeNet;
            }
        }
    }
}
