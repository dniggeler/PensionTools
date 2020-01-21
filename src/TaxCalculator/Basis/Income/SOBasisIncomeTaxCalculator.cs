using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator.Basis.Income
{
    /// <summary>
    /// Basis calculator for SO.
    /// </summary>
    /// <seealso cref="PensionCoach.Tools.TaxCalculator.Abstractions.IBasisIncomeTaxCalculator" />
    public class SOBasisIncomeTaxCalculator : IBasisIncomeTaxCalculator
    {
        private const int IncomeTaxTypeId = (int)TaxType.Income;

        private readonly IDefaultBasisIncomeTaxCalculator defaultBasisIncomeTaxCalculator;

        public SOBasisIncomeTaxCalculator(
            IDefaultBasisIncomeTaxCalculator defaultBasisIncomeTaxCalculator)
        {
            this.defaultBasisIncomeTaxCalculator = defaultBasisIncomeTaxCalculator;
        }

        public async Task<Either<string, BasisTaxResult>> CalculateAsync(
            int calculationYear, Canton canton, BasisTaxPerson person)
        {
            const decimal splittingFactor = 1.9M;

            // this canton does not have a tariff of its own
            // for married people but the following rule applies:
            // divide taxable income by the splitting factor if married,
            // calculate basis tax and multiply by splitting factor again.
            // This method breaks the progression and results in a lower tax
            // amount.
            (decimal TaxAmount, decimal Multiplier) adaptedTaxData =
                person.CivilStatus
                    .Match(
                        Some: status => status switch
                        {
                            CivilStatus.Married => (person.TaxableAmount / splittingFactor, splittingFactor),
                            _ => (person.TaxableAmount, 1M)
                        },
                        None: () => (person.TaxableAmount, 1));

            var tmpPerson = new BasisTaxPerson
            {
                Name = person.Name,
                CivilStatus = CivilStatus.Single,
                TaxableAmount = adaptedTaxData.TaxAmount,
            };

            var taxResult =
                await this.defaultBasisIncomeTaxCalculator
                    .CalculateAsync(calculationYear, canton, tmpPerson);

            taxResult
                .IfRight(r =>
                {
                    r.TaxAmount = r.TaxAmount * adaptedTaxData.Multiplier;
                    r.DeterminingFactorTaxableAmount =
                        r.DeterminingFactorTaxableAmount * adaptedTaxData.Multiplier;
                });

            return taxResult;
        }
    }
}
