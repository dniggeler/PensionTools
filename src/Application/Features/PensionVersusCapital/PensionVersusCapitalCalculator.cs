using Application.Features.FullTaxCalculation;
using Domain.Enums;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Features.PensionVersusCapital;

public class PensionVersusCapitalCalculator : IPensionVersusCapitalCalculator
{
    private readonly ITaxCalculatorConnector taxCalculatorConnector;

    public PensionVersusCapitalCalculator(ITaxCalculatorConnector taxCalculatorConnector)
    {
        this.taxCalculatorConnector = taxCalculatorConnector;
    }

    public async Task<Option<decimal>> CalculateAsync(
        int calculationYear,
        int municipalityId,
        Canton canton,
        decimal retirementPension,
        decimal retirementCapital,
        TaxPerson taxPerson)
    {
        CapitalBenefitTaxPerson capitalBenefitTaxPerson = new()
        {
            Name = "Benefit Person",
            CivilStatus = taxPerson.CivilStatus,
            TaxableCapitalBenefits = retirementCapital,
            ReligiousGroupType = taxPerson.ReligiousGroupType,
            NumberOfChildren = taxPerson.NumberOfChildren,
            PartnerReligiousGroupType = taxPerson.PartnerReligiousGroupType
        };

        Either<string, FullTaxResult> justOtherIncomeTaxCalculationResult =
            await taxCalculatorConnector.CalculateAsync(calculationYear, municipalityId, taxPerson, true);

        Either<string, FullCapitalBenefitTaxResult> capitalBenefitTaxCalculationResult =
            await taxCalculatorConnector.CalculateAsync(calculationYear, municipalityId, capitalBenefitTaxPerson, true);

        var taxPersonWithPensionIncome = taxPerson with
        {
            TaxableIncome = taxPerson.TaxableIncome + retirementPension,
            TaxableFederalIncome = taxPerson.TaxableFederalIncome + retirementPension,
        };

        Either<string, FullTaxResult> withPensionIncomeTaxCalculationResult =
            await taxCalculatorConnector.CalculateAsync(calculationYear, municipalityId, taxPersonWithPensionIncome, true);

        var r = from benefitsTax in capitalBenefitTaxCalculationResult
            from otherIncomeTax in justOtherIncomeTaxCalculationResult
            from withPensionIncomeTax in withPensionIncomeTaxCalculationResult
            select CalculateBreakEvenFactor(benefitsTax, otherIncomeTax, withPensionIncomeTax);

        return r.IfLeft(()=>Option<decimal>.None);

        Option<decimal> CalculateBreakEvenFactor(
            FullCapitalBenefitTaxResult benefitTaxResult,
            FullTaxResult otherIncomeTaxResult,
            FullTaxResult withPensionIncomeTaxResult)
        {
            decimal capitalBenefitNet = capitalBenefitTaxPerson.TaxableCapitalBenefits - benefitTaxResult.TotalTaxAmount;
            decimal incomeNet = taxPersonWithPensionIncome.TaxableIncome - taxPerson.TaxableIncome;
            decimal totalTaxNet = withPensionIncomeTaxResult.TotalTaxAmount - otherIncomeTaxResult.TotalTaxAmount;

            if (incomeNet == decimal.Zero)
            {
                return Option<decimal>.None;
            }

            return capitalBenefitNet / (incomeNet - totalTaxNet);
        }
    }
}
