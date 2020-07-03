using System.Threading.Tasks;
using LanguageExt;
using Microsoft.Extensions.Logging;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator.Basis.Income
{
    /// <summary>
    /// Null basis income calculator for missing cantons.
    /// </summary>
    /// <seealso cref="PensionCoach.Tools.TaxCalculator.Abstractions.IBasisIncomeTaxCalculator" />
    public class MissingBasisIncomeTaxCalculator : IBasisIncomeTaxCalculator
    {
        private readonly ILogger<MissingBasisIncomeTaxCalculator> logger;

        public MissingBasisIncomeTaxCalculator(ILogger<MissingBasisIncomeTaxCalculator> logger)
        {
            this.logger = logger;
        }

        public Task<Either<string, BasisTaxResult>> CalculateAsync(
            int calculationYear, Canton canton, BasisTaxPerson person)
        {
            string msg = $"No income tax calculator for canton {canton.ToString()} available";

            Either<string, BasisTaxResult> result = msg;

            this.logger.LogWarning(msg);

            return Task.FromResult(result);
        }
    }
}
