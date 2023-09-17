using System.Threading.Tasks;
using Domain.Enums;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;


namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface ICapitalBenefitTaxCalculator
    {
        Task<Either<string, CapitalBenefitTaxResult>> CalculateAsync(
            int calculationYear,
            int taxId,
            Canton canton,
            CapitalBenefitTaxPerson person);
    }
}
