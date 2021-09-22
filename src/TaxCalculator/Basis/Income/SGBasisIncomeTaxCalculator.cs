using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator.Basis.Income
{
    /// <summary>
    /// Basis calculator for SG.
    /// </summary>
    /// <seealso cref="PensionCoach.Tools.TaxCalculator.Abstractions.IBasisIncomeTaxCalculator" />
    public class SGBasisIncomeTaxCalculator : IBasisIncomeTaxCalculator
    {
        private readonly IDefaultBasisIncomeTaxCalculator defaultBasisIncomeTaxCalculator;

        public SGBasisIncomeTaxCalculator(
            IDefaultBasisIncomeTaxCalculator defaultBasisIncomeTaxCalculator)
        {
            this.defaultBasisIncomeTaxCalculator = defaultBasisIncomeTaxCalculator;
        }

        public async Task<Either<string, BasisTaxResult>> CalculateAsync(
            int calculationYear, Canton canton, BasisTaxPerson person)
        {
            // this canton does not have a tariff of its own
            // for married people but the following rule applies:
            // divide taxable income by 2 if married and
            // multiple by 2 (to break the progression)
            (decimal TaxAmount, decimal Multiplier) adaptedTaxData =
                Prelude.Some(person.CivilStatus)
                    .Match(
                        Some: status => status switch
                        {
                            CivilStatus.Married => (person.TaxableAmount / 2M, 2M),
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
                await defaultBasisIncomeTaxCalculator.CalculateAsync(calculationYear, canton, tmpPerson);

            taxResult
                .IfRight(r =>
                {
                    r.TaxAmount *= adaptedTaxData.Multiplier;
                    r.DeterminingFactorTaxableAmount *= adaptedTaxData.Multiplier;
                });

            return taxResult;
        }
    }
}
