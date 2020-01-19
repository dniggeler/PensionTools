using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;


namespace TaxCalculator.Basis.CapitalBenefit
{
    /// <summary>
    /// Null calculator for missing capital benefit calculator.
    /// </summary>
    public class MissingCapitalBenefitTaxCalculator : ICapitalBenefitTaxCalculator
    {
        private readonly ILogger<MissingCapitalBenefitTaxCalculator> logger;

        public MissingCapitalBenefitTaxCalculator(ILogger<MissingCapitalBenefitTaxCalculator> logger)
        {
            this.logger = logger;
        }

        public Task<Either<string, CapitalBenefitTaxResult>> CalculateAsync(int calculationYear, int municipalityId, Canton canton, CapitalBenefitTaxPerson person)
        {
            string msg = $"No capital benefit tax calculator for canton {canton.ToString()} available";

            Either<string, CapitalBenefitTaxResult> result = msg;

            this.logger.LogWarning(msg);

            return Task.FromResult(result);
        }
    }
}
