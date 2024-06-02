using Application.Municipality;
using Application.Tax.Contracts;
using Application.Tax.Proprietary.Models;
using Domain.Models.Municipality;
using Domain.Models.Tax;
using LanguageExt;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Features.MarginalTaxCurve;

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
                .MapAsync(model => CalculateInternalAsync(model, person)))
            .Iter(t => result.MarginalTaxCurve = t);

        (await municipalityResult
                .MapAsync(municipalityModel => CalculateSingleMarginalTaxRate(municipalityModel, person)))
            .Iter(taxRate => taxRate.Iter(rate => result.CurrentMarginalTaxRate = rate));

        Merge(result);

        return result;

        void Merge(MarginalTaxCurveResult beforeMergeResult)
        {
            if (beforeMergeResult.CurrentMarginalTaxRate is null)
            {
                return;
            }

            if (beforeMergeResult.MarginalTaxCurve.All(p =>
                    p.Salary != beforeMergeResult.CurrentMarginalTaxRate.Salary))
            {
                beforeMergeResult.MarginalTaxCurve.Add(beforeMergeResult.CurrentMarginalTaxRate);
            }
        }

        async Task<IList<MarginalTaxInfo>> CalculateInternalAsync(
            MunicipalityModel municipalityModel, TaxPerson taxPerson)
        {
            List<MarginalTaxInfo> incomeTaxes = new();

            decimal previousMarginalTaxRate = 0;
            foreach (decimal currentSalary in LinearSpace(lowerLimit, upperLimit, numberOfSamples))
            {
                var currentPerson = taxPerson with { TaxableIncome = currentSalary, TaxableFederalIncome = currentSalary };

                (await CalculateSingleMarginalTaxRate(municipalityModel, currentPerson))
                    .Iter(r =>
                    {
                        if (r.Rate > previousMarginalTaxRate)
                        {
                            incomeTaxes.Add(new MarginalTaxInfo(r.Salary, r.Rate, r.TotalTaxAmount));
                            previousMarginalTaxRate = r.Rate;
                        }
                    });
            }

            return incomeTaxes;
        }

        async Task<Either<string, MarginalTaxInfo>> CalculateSingleMarginalTaxRate(
            MunicipalityModel municipalityModel, TaxPerson taxPerson)
        {
            const decimal delta = 1000M;

            var x0Person = person with
            {
                TaxableIncome = taxPerson.TaxableIncome, TaxableFederalIncome = taxPerson.TaxableFederalIncome
            };

            Either<string, FullTaxResult> tax0 =
                await fullWealthAndIncomeTaxCalculator.CalculateAsync(calculationYear, municipalityModel, x0Person);

            var x1Person = taxPerson with
            {
                TaxableIncome = taxPerson.TaxableIncome + delta,
                TaxableFederalIncome = taxPerson.TaxableFederalIncome + delta
            };

            Either<string, FullTaxResult> tax1 =
                await fullWealthAndIncomeTaxCalculator.CalculateAsync(calculationYear, municipalityModel, x1Person);

            var r =
                from t0 in tax0
                from t1 in tax1
                select new MarginalTaxInfo(
                    taxPerson.TaxableIncome,
                    (t1.TotalTaxAmount - t0.TotalTaxAmount) / delta,
                    t0.TotalTaxAmount);

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
                .MapAsync(model => CalculateInternalAsync(model, person)))
            .Iter(t => result.MarginalTaxCurve = t);

        (await municipalityResult
                .MapAsync(model => CalculateSingleMarginalTaxRate(model, person)))
            .Iter(taxRate => taxRate.Iter(rate => result.CurrentMarginalTaxRate = rate));

        // Create pairs of tax rates from the curve and check if the current tax rate is between them.
        // If it is, adjust the current tax rate to the one from the curve.
        foreach (var pair in result.MarginalTaxCurve.Zip(result.MarginalTaxCurve.Skip(1)))
        {
            if (pair.Item1.Salary <= result.CurrentMarginalTaxRate.Salary &&
                pair.Item2.Salary > result.CurrentMarginalTaxRate.Salary)
            {
                result.CurrentMarginalTaxRate = result.CurrentMarginalTaxRate with { Rate = pair.Item1.Rate };
            }
        }


        return result;

        async Task<Either<string, MarginalTaxInfo>> CalculateSingleMarginalTaxRate(
            MunicipalityModel municipalityModel, CapitalBenefitTaxPerson taxPerson)
        {
            decimal delta = 1000M;

            Either<string, FullCapitalBenefitTaxResult> tax0 =
                await fullCapitalBenefitTaxCalculator.CalculateAsync(calculationYear, municipalityModel, taxPerson);

            var x1Person = taxPerson with { TaxableCapitalBenefits = taxPerson.TaxableCapitalBenefits + delta };

            Either<string, FullCapitalBenefitTaxResult> tax1 =
                await fullCapitalBenefitTaxCalculator.CalculateAsync(calculationYear, municipalityModel, x1Person);

            Either<string, MarginalTaxInfo> r =
                from t0 in tax0
                from t1 in tax1
                select new MarginalTaxInfo(
                    taxPerson.TaxableCapitalBenefits,
                    (t1.TotalTaxAmount - t0.TotalTaxAmount) / delta,
                    t0.TotalTaxAmount);

            return r;
        }

        async Task<IList<MarginalTaxInfo>> CalculateInternalAsync(
            MunicipalityModel municipalityModel, CapitalBenefitTaxPerson taxPerson)
        {
            int currentSalary = (int)taxPerson.TaxableCapitalBenefits;
            int[] linearSpace = LinearSpace(lowerLimit, upperLimit, numberOfSamples);
            int currentValueIndex = SortedIndex(linearSpace, currentSalary);
            linearSpace = InsertAt(linearSpace, (int)taxPerson.TaxableCapitalBenefits, currentValueIndex);

            List<MarginalTaxInfo> taxes = new();

            decimal previousMarginalTaxRate = 0;
            foreach (decimal salary in linearSpace)
            {
                var currentPerson = taxPerson with { TaxableCapitalBenefits = salary };

                (await CalculateSingleMarginalTaxRate(municipalityModel, currentPerson))
                    .Iter(r =>
                    {
                        if (r.Rate <= previousMarginalTaxRate)
                        {
                            if (taxes.Count == 0)
                            {
                                taxes.Add(new MarginalTaxInfo(r.Salary, r.Rate, r.TotalTaxAmount));
                                previousMarginalTaxRate = r.Rate;
                            }
                            else
                            {
                                taxes.Add(taxes[^1] with { Salary = r.Salary, TotalTaxAmount = r.TotalTaxAmount });
                            }
                        }
                        else if (r.Rate > previousMarginalTaxRate)
                        {
                            taxes.Add(new MarginalTaxInfo(r.Salary, r.Rate, r.TotalTaxAmount));
                            previousMarginalTaxRate = r.Rate;
                        }
                    });
            }

            return taxes;
        }
    }

    protected static int[] LinearSpace(int start, int end, int size)
    {
        int[] result = new int[size];
        decimal step = (end - start) / (size - 1M);

        for (int i = 0; i < size; i++)
        {
            result[i] = (int)(start + (i * step));
        }

        // Ensure the end value is exactly as specified, avoiding floating-point arithmetic errors
        if (size > 1)
        {
            result[size - 1] = end;
        }
        
        return result;
    }

    private static int SortedIndex(int[] array, int value)
    {
        int low = 0;
        int high = array.Length;

        while (low < high)
        {
            var mid = (low + high) >>> 1;
            if (array[mid] < value) low = mid + 1;
            else high = mid;
        }

        return low;
    }

    private static int[] InsertAt(int[] originalArray, int value, int index)
    {
        // Handle the case where index is out of bounds
        if (index < 0 || index > originalArray.Length)
        {
            index = originalArray.Length; // Append to the end if index is out of bounds
        }
        else if (originalArray[index] == value)
        {
            return originalArray;
        }

        // Create a new array with one extra space
        int[] newArray = new int[originalArray.Length + 1];

        for (int i = 0, j = 0; i < newArray.Length; i++)
        {
            if (i == index)
            {
                // Insert the new value at the specified index
                newArray[i] = value;
            }
            else
            {
                // Copy the value from the original array
                newArray[i] = originalArray[j++];
            }
        }

        return newArray;
    }
}
