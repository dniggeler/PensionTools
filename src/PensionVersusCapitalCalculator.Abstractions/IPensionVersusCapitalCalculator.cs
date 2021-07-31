using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace PensionVersusCapitalCalculator.Abstractions
{
    public interface IPensionVersusCapitalCalculator
    {
        Task<Option<decimal>> CalculateAsync(
            int calculationYear,
            int municipalityId,
            Canton canton,
            TaxPerson incomeTaxPerson,
            CapitalBenefitTaxPerson capitalBenefitTaxPerson);
    }
}
