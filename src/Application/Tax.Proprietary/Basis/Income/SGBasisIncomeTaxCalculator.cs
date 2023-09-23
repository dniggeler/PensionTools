using Application.Tax.Proprietary.Abstractions.Models;
using Application.Tax.Proprietary.Contracts;
using Domain.Enums;
using Domain.Models.Tax.Person;
using LanguageExt;

namespace Application.Tax.Proprietary.Basis.Income;

/// <summary>
/// Basis calculator for SG.
/// </summary>
/// <seealso cref="IBasisIncomeTaxCalculator" />
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
