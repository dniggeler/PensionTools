using System.Text;
using Application.Tax.Proprietary.Contracts;
using AutoMapper;
using Domain.Enums;
using Domain.Models.Municipality;
using Domain.Models.Tax;
using Domain.Models.Tax.Person;
using LanguageExt;

namespace Application.Tax.Proprietary;

public class ProprietaryFullCapitalBenefitTaxCalculator : IFullCapitalBenefitTaxCalculator
{
    private readonly Func<Canton, ICapitalBenefitTaxCalculator> capitalBenefitCalculatorFunc;
    private readonly IFederalCapitalBenefitTaxCalculator federalCalculator;
    private readonly IMapper mapper;

    public ProprietaryFullCapitalBenefitTaxCalculator(
        Func<Canton, ICapitalBenefitTaxCalculator> capitalBenefitCalculatorFunc,
        IFederalCapitalBenefitTaxCalculator federalCalculator,
        IMapper mapper)
    {
        this.capitalBenefitCalculatorFunc = capitalBenefitCalculatorFunc;
        this.federalCalculator = federalCalculator;
        this.mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<Either<string, FullCapitalBenefitTaxResult>> CalculateAsync(
        int calculationYear,
        MunicipalityModel municipality,
        CapitalBenefitTaxPerson capitalBenefitTaxPerson,
        bool withMaxAvailableCalculationYear)
    {
        var maxCalculationYear = withMaxAvailableCalculationYear
            ? Math.Min(calculationYear, 2019)
            : calculationYear;

        var capitalBenefitTaxResultTask =
            capitalBenefitCalculatorFunc(municipality.Canton)
                .CalculateAsync(maxCalculationYear, municipality.BfsNumber, municipality.Canton, capitalBenefitTaxPerson);

        var federalTaxPerson = mapper.Map<FederalTaxPerson>(capitalBenefitTaxPerson);

        var federalTaxResultTask = federalCalculator.CalculateAsync(maxCalculationYear, federalTaxPerson);

        await Task.WhenAll(capitalBenefitTaxResultTask, federalTaxResultTask);

        var sb = new StringBuilder();

        var stateTaxResult = await capitalBenefitTaxResultTask;
        var federalTaxResult = await federalTaxResultTask;

        stateTaxResult.MapLeft(r => sb.AppendLine(r));
        federalTaxResult.MapLeft(r => sb.AppendLine(r));

        var fullResult =
            from s in stateTaxResult.ToOption()
            from f in federalTaxResult.ToOption()
            select new FullCapitalBenefitTaxResult
            {
                StateResult = s,
                FederalResult = f,
            };

        return fullResult
            .Match<Either<string, FullCapitalBenefitTaxResult>>(
                Some: r => r,
                None: () => sb.ToString());
    }
}
