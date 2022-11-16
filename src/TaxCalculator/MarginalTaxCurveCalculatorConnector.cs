using System.Collections.Generic;
using System.Linq;
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

        (await municipalityResolver
                .GetAsync(bfsMunicipalityId, calculationYear)
                .MapAsync(CalculateInternalAsync))
            .Map(CalculateMarginalTaxRates)
            .Iter(t => result.MarginalTaxCurve = t);

        return result;

        async Task<Dictionary<int, decimal>> CalculateInternalAsync(
            MunicipalityModel municipalityModel)
        {
            int stepSize = (upperLimit - lowerLimit) / numberOfSamples;

            Dictionary<int, decimal> incomeTaxes = new Dictionary<int, decimal>();

            int currentSalary = lowerLimit;

            while (currentSalary <= upperLimit)
            {
                TaxPerson currentPerson = person with
                {
                    TaxableIncome = currentSalary,
                    TaxableFederalIncome = currentSalary
                };

                int salary = currentSalary;
                (await fullWealthAndIncomeTaxCalculator.CalculateAsync(
                    calculationYear, municipalityModel, currentPerson))
                    .Iter(r =>
                    {
                        incomeTaxes.Add(salary, r.TotalTaxAmount);
                    });

                currentSalary += stepSize;
            }

            return incomeTaxes;
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

        (await municipalityResolver
                .GetAsync(bfsMunicipalityId, calculationYear)
                .MapAsync(CalculateInternalAsync))
            .Map(CalculateMarginalTaxRates)
            .Iter(t => result.MarginalTaxCurve = t);

        return result;

        async Task<Dictionary<int, decimal>> CalculateInternalAsync(
            MunicipalityModel municipalityModel)
        {
            int stepSize = (upperLimit - lowerLimit) / numberOfSamples;

            Dictionary<int, decimal> incomeTaxes = new Dictionary<int, decimal>();

            int currentSalary = lowerLimit;

            while (currentSalary <= upperLimit)
            {
                CapitalBenefitTaxPerson currentPerson = person with { TaxableCapitalBenefits = currentSalary };

                int salary = currentSalary;
                (await fullCapitalBenefitTaxCalculator.CalculateAsync(
                    calculationYear, municipalityModel, currentPerson))
                    .Iter(r =>
                    {
                        incomeTaxes.Add(salary, r.TotalTaxAmount);
                    });

                currentSalary += stepSize;
            }

            return incomeTaxes;
        }
    }

    private Dictionary<int, decimal> CalculateMarginalTaxRates(Dictionary<int, decimal> taxAmounts)
    {
        Dictionary<int, decimal> marginalTaxRates = new();

        if (taxAmounts is null || taxAmounts.Count is 0 or 1)
        {
            return marginalTaxRates;
        }

        marginalTaxRates.Add(0, decimal.Zero);

        var predecessorPair = taxAmounts.First();

        foreach (var pair in taxAmounts.Skip(1))
        {
            int dS = pair.Key - predecessorPair.Key;

            if (dS > 0)
            {
                decimal dF = pair.Value - predecessorPair.Value;
                marginalTaxRates.Add(pair.Key, dF / dS);
            }
            predecessorPair = pair;
        }

        return marginalTaxRates;
    }
}
