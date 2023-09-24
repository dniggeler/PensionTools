using Application.Tax.Proprietary.Models;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Features.MarginalTaxCurve;

public interface IMarginalTaxCurveCalculatorConnector
{
    Task<Either<string, MarginalTaxCurveResult>> CalculateIncomeTaxCurveAsync(
        int calculationYear,
        int bfsMunicipalityId,
        TaxPerson person,
        int lowerLimit,
        int upperLimit,
        int numberOfSamples);

    Task<Either<string, MarginalTaxCurveResult>> CalculateCapitalBenefitTaxCurveAsync(
        int calculationYear,
        int bfsMunicipalityId,
        CapitalBenefitTaxPerson person,
        int lowerLimit,
        int upperLimit,
        int numberOfSamples);

}
