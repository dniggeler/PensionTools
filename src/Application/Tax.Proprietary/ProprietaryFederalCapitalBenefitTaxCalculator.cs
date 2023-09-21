using Application.Tax.Proprietary.Abstractions;
using Application.Tax.Proprietary.Abstractions.Models;
using Application.Tax.Proprietary.Abstractions.Models.Person;
using AutoMapper;
using LanguageExt;

namespace Application.Tax.Proprietary;

public class ProprietaryFederalCapitalBenefitTaxCalculator : IFederalCapitalBenefitTaxCalculator
{
    private readonly IFederalTaxCalculator taxCalculator;
    private readonly IMapper mapper;

    public ProprietaryFederalCapitalBenefitTaxCalculator(
        IFederalTaxCalculator taxCalculator,
        IMapper mapper)
    {
        this.taxCalculator = taxCalculator;
        this.mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<Either<string, BasisTaxResult>> CalculateAsync(
        int calculationYear, FederalTaxPerson capitalBenefitTaxPerson)
    {
        const decimal scaleFactor = 0.2M;
        var taxPerson = mapper.Map<FederalTaxPerson>(capitalBenefitTaxPerson);

        var taxResult = await taxCalculator.CalculateAsync(calculationYear, taxPerson);

        return taxResult.Map(r => Scale(r, scaleFactor));
    }

    private BasisTaxResult Scale(BasisTaxResult intermediateResult, decimal scaleFactor)
    {
        return new BasisTaxResult
        {
            DeterminingFactorTaxableAmount =
                intermediateResult.DeterminingFactorTaxableAmount,
            TaxAmount =
                intermediateResult.TaxAmount * scaleFactor,
        };
    }
}
