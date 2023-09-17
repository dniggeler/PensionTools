using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models.Tax;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface IWealthTaxCalculator
    {
        Task<Either<string, SingleTaxResult>> CalculateAsync(
            int calculationYear, int municipalityId, Canton canton, TaxPerson person);
    }
}
