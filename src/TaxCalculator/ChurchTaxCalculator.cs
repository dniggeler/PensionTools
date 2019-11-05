namespace TaxCalculator
{
    using System.Threading.Tasks;
    using LanguageExt;
    using PensionCoach.Tools.TaxCalculator.Abstractions;
    using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

    /// <inheritdoc />
    public class ChurchTaxCalculator : IChurchTaxCalculator
    {
        /// <inheritdoc />
        public Task<Either<string, ChurchTaxResult>> CalculateAsync(
            int calculationYear, TaxPerson person, BasisTaxResult basisIncomeTaxResult)
        {
            Either<string, ChurchTaxResult> churchTaxResult = new ChurchTaxResult
            {
                TaxAmount = 117M,
                TaxAmountPartner =  117M,
            };

            return Task.FromResult(churchTaxResult);
        }
    }
}