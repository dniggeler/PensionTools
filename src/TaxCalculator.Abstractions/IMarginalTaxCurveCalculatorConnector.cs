using LanguageExt;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using System.Threading.Tasks;

namespace PensionCoach.Tools.TaxCalculator.Abstractions;

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
