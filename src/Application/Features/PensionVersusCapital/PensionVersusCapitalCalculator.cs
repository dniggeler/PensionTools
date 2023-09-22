using Application.Tax.Proprietary.Abstractions;
using Application.Tax.Proprietary.Abstractions.Models;
using Domain.Enums;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Features.PensionVersusCapital;

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
        int calculationYear,
        int municipalityId,
        Canton canton,
        decimal retirementPension,
        decimal retirementCapital,
        TaxPerson taxPerson)
    {
        ICapitalBenefitTaxCalculator capitalBenefitCalculator = capitalBenefitCalculatorFunc(canton);

        CapitalBenefitTaxPerson capitalBenefitTaxPerson = new()
        {
            Name = "Benefit Person",
            CivilStatus = taxPerson.CivilStatus,
            TaxableCapitalBenefits = retirementCapital,
            ReligiousGroupType = taxPerson.ReligiousGroupType,
            NumberOfChildren = taxPerson.NumberOfChildren,
            PartnerReligiousGroupType = taxPerson.PartnerReligiousGroupType
        };
            
        Either<string, CapitalBenefitTaxResult> capitalBenefitTaxCalculationResult =
            await capitalBenefitCalculator.CalculateAsync(calculationYear, municipalityId, canton, capitalBenefitTaxPerson);

        Either<string, SingleTaxResult> justOtherIncomeTaxCalculationResult =
            await incomeTaxCalculator.CalculateAsync(calculationYear, municipalityId, canton, taxPerson);

        decimal otherIncome = taxPerson.TaxableIncome;

        taxPerson.TaxableIncome += retirementPension;
        taxPerson.TaxableFederalIncome += retirementPension;

        Either<string, SingleTaxResult> withPensionIncomeTaxCalculationResult =
            await incomeTaxCalculator.CalculateAsync(calculationYear, municipalityId, canton, taxPerson);

        var r = from benefitsTax in capitalBenefitTaxCalculationResult
            from otherIncomeTax in justOtherIncomeTaxCalculationResult
            from withPensionIncomeTax in withPensionIncomeTaxCalculationResult
            select CalculateBreakEvenFactor(benefitsTax, otherIncomeTax, withPensionIncomeTax);

        return r.IfLeft(()=>Option<decimal>.None);

        Option<decimal> CalculateBreakEvenFactor(
            CapitalBenefitTaxResult benefitTaxResult,
            SingleTaxResult otherIncomeTaxResult,
            SingleTaxResult withPensionIncomeTaxResult)
        {
            decimal capitalBenefitNet = capitalBenefitTaxPerson.TaxableCapitalBenefits - benefitTaxResult.TotalTaxAmount;
            decimal incomeNet = taxPerson.TaxableIncome - otherIncome;
            decimal totalTaxNet = withPensionIncomeTaxResult.TotalTaxAmount - otherIncomeTaxResult.TotalTaxAmount;

            if (incomeNet == decimal.Zero)
            {
                return Option<decimal>.None;
            }

                

            return capitalBenefitNet / (incomeNet - totalTaxNet);
        }
    }
}
