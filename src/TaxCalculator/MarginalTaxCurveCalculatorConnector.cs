using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace PensionCoach.Tools.TaxCalculator;

public class MarginalTaxCurveCalculatorConnector : IMarginalTaxCurveCalculatorConnector
{
    private readonly IFullWealthAndIncomeTaxCalculator fullWealthAndIncomeTaxCalculator;
    private readonly IFullCapitalBenefitTaxCalculator fullCapitalBenefitTaxCalculator;
    private readonly IMunicipalityConnector municipalityResolver;

    public MarginalTaxCurveCalculatorConnector(
        IFullWealthAndIncomeTaxCalculator fullWealthAndIncomeTaxCalculator,
        IFullCapitalBenefitTaxCalculator fullCapitalBenefitTaxCalculator,
        IMunicipalityConnector municipalityResolver)
    {
        this.fullWealthAndIncomeTaxCalculator = fullWealthAndIncomeTaxCalculator;
        this.fullCapitalBenefitTaxCalculator = fullCapitalBenefitTaxCalculator;
        this.municipalityResolver = municipalityResolver;
    }

    public async Task<Either<string, MarginalTaxCurveResult>> CalculateIncomeTaxCurveAsync(
        int calculationYear,
        int bfsMunicipalityId,
        TaxPerson person,
        int lowerLimit,
        int upperLimit,
        int numberOfSamples)
    {
        MarginalTaxCurveResult result = new();

        Either<string, MunicipalityModel> municipalityResult = await municipalityResolver
            .GetAsync(bfsMunicipalityId, calculationYear);

        (await municipalityResult
                .MapAsync(CalculateInternalAsync))
            .Iter(t => result.MarginalTaxCurve = t);

        (await municipalityResult
                .MapAsync(municipalityModel => CalculateSingleMarginalTaxRate(municipalityModel, person)))
            .Iter(taxRate => taxRate.Iter(rate => result.CurrentMarginalTaxRate = rate));

        return result;

        async Task<Dictionary<decimal, decimal>> CalculateInternalAsync(
            MunicipalityModel municipalityModel)
        {
            int stepSize = (upperLimit - lowerLimit) / numberOfSamples;

            Dictionary<decimal, decimal> incomeTaxes = new();

            int currentSalary = lowerLimit;

            while (currentSalary <= upperLimit)
            {
                var currentPerson = person with
                {
                    TaxableIncome = currentSalary,
                    TaxableFederalIncome = currentSalary
                };

                (await CalculateSingleMarginalTaxRate(municipalityModel, currentPerson))
                    .Iter(r =>
                    {
                        incomeTaxes.Add(r.Amount, r.Rate);
                    });

                currentSalary += stepSize;
            }

            return incomeTaxes;
        }

        async Task<Either<string, MarginalTaxCurveResult.MarginalTaxRate>> CalculateSingleMarginalTaxRate(
            MunicipalityModel municipalityModel, TaxPerson taxPerson)
        {
            decimal delta = 100M;

            Either<string, FullTaxResult> tax0 =
                await fullWealthAndIncomeTaxCalculator.CalculateAsync(calculationYear, municipalityModel, taxPerson);

            var x1Person = taxPerson with
            {
                TaxableIncome = taxPerson.TaxableIncome + delta,
                TaxableFederalIncome = taxPerson.TaxableFederalIncome + delta
            };

            Either<string, FullTaxResult> tax1 =
                await fullWealthAndIncomeTaxCalculator.CalculateAsync(calculationYear, municipalityModel, x1Person);

            var r = from t0 in tax0
                from t1 in tax1
                select new MarginalTaxCurveResult.MarginalTaxRate(
                    taxPerson.TaxableIncome,
                    (t1.TotalTaxAmount - t0.TotalTaxAmount) / delta);

            return r;
        }
    }

    public async Task<Either<string, MarginalTaxCurveResult>> CalculateCapitalBenefitTaxCurveAsync(
        int calculationYear,
        int bfsMunicipalityId,
        CapitalBenefitTaxPerson person,
        int lowerLimit,
        int upperLimit,
        int numberOfSamples)
    {
        MarginalTaxCurveResult result = new();

        Either<string, MunicipalityModel> municipalityResult = await municipalityResolver
            .GetAsync(bfsMunicipalityId, calculationYear);

        (await municipalityResult
                .MapAsync(CalculateInternalAsync))
            .Iter(t => result.MarginalTaxCurve = t);

        (await municipalityResult
                .MapAsync(model => CalculateSingleMarginalTaxRate(model, person)))
            .Iter(taxRate => taxRate.Iter(rate => result.CurrentMarginalTaxRate = rate));

        return result;

        async Task<Either<string, MarginalTaxCurveResult.MarginalTaxRate>> CalculateSingleMarginalTaxRate(
            MunicipalityModel municipalityModel, CapitalBenefitTaxPerson taxPerson)
        {
            decimal delta = 1000M;

            Either<string, FullCapitalBenefitTaxResult> tax0 =
                await fullCapitalBenefitTaxCalculator.CalculateAsync(calculationYear, municipalityModel, taxPerson);

            var x1Person = taxPerson with { TaxableCapitalBenefits = taxPerson.TaxableCapitalBenefits + delta };

            Either<string, FullCapitalBenefitTaxResult> tax1 =
                await fullCapitalBenefitTaxCalculator.CalculateAsync(calculationYear, municipalityModel, x1Person);

            Either<string, MarginalTaxCurveResult.MarginalTaxRate> r = from t0 in tax0
                from t1 in tax1
                select new MarginalTaxCurveResult.MarginalTaxRate(
                    taxPerson.TaxableCapitalBenefits,
                    (t1.TotalTaxAmount - t0.TotalTaxAmount) / delta);

            return r;
        }

        async Task<Dictionary<decimal, decimal>> CalculateInternalAsync(
            MunicipalityModel municipalityModel)
        {
            int stepSize = (upperLimit - lowerLimit) / numberOfSamples;

            Dictionary<decimal, decimal> incomeTaxes = new();

            int currentSalary = lowerLimit;

            while (currentSalary <= upperLimit)
            {
                var currentPerson = person with
                {
                    TaxableCapitalBenefits = currentSalary
                };

                (await CalculateSingleMarginalTaxRate(municipalityModel, currentPerson))
                    .Iter(r =>
                    {
                        incomeTaxes.Add(r.Amount, r.Rate);
                    });

                currentSalary += stepSize;
            }

            return incomeTaxes;
        }
    }
}
