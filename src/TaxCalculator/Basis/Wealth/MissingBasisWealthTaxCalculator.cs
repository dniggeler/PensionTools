using System.Threading.Tasks;
using Domain.Enums;
using LanguageExt;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace PensionCoach.Tools.TaxCalculator.Basis.Wealth
{
    /// <summary>
    /// Null calculator for missing wealth calculators.
    /// </summary>
    public class MissingBasisWealthTaxCalculator : IBasisWealthTaxCalculator
    {
        private readonly ILogger<MissingBasisWealthTaxCalculator> logger;

        public MissingBasisWealthTaxCalculator(ILogger<MissingBasisWealthTaxCalculator> logger)
        {
            this.logger = logger;
        }

        public Task<Either<string, BasisTaxResult>> CalculateAsync(
            int calculationYear, Canton canton, BasisTaxPerson person)
        {
            string msg = $"No wealth tax calculator for canton {canton.ToString()} available";

            Either<string, BasisTaxResult> result = msg;

            logger.LogWarning(msg);

            return Task.FromResult(result);
        }
    }
}
