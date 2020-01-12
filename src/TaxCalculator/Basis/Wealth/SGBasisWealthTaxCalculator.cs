using System;
using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;

namespace TaxCalculator.Basis.Wealth
{
    public class SGBasisWealthTaxCalculator : IBasisWealthTaxCalculator
    {
        private const decimal TaxRate = 1.17M / 1000M;
        private const decimal MinLevel = 1000M;

        public Task<Either<string, BasisTaxResult>> CalculateAsync(int calculationYear, Canton canton, BasisTaxPerson person)
        {
            Either<string, BasisTaxResult> taxResult = new BasisTaxResult();

            if (person.TaxableAmount < MinLevel)
            {
                return taxResult.AsTask();
            }

            taxResult.IfRight(r =>
            {
                r.TaxAmount = r.TaxAmount * TaxRate;
                r.DeterminingFactorTaxableAmount = r.TaxAmount;
            });

            return taxResult.AsTask();
        }
    }
}