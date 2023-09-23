using Application.Tax.Proprietary.Abstractions.Models;
using Application.Tax.Proprietary.Abstractions.Models.Person;
using Application.Tax.Proprietary.Contracts;
using Domain.Enums;
using LanguageExt;

namespace Application.Tax.Proprietary.Basis.Wealth;

public class SGBasisWealthTaxCalculator : IBasisWealthTaxCalculator
{
    private const decimal TaxRate = 1.7M / 1000M;
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
            r.TaxAmount = person.TaxableAmount * TaxRate;
            r.DeterminingFactorTaxableAmount = person.TaxableAmount;
        });

        return taxResult.AsTask();
    }
}
