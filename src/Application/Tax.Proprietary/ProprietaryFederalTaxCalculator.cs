using Application.Tax.Proprietary.Abstractions.Models;
using Application.Tax.Proprietary.Abstractions.Models.Person;
using Application.Tax.Proprietary.Abstractions.Repositories;
using Application.Tax.Proprietary.Contracts;
using Domain.Enums;
using Domain.Models.Tax;
using FluentValidation;
using FluentValidation.Results;
using LanguageExt;

namespace Application.Tax.Proprietary;

public class ProprietaryFederalTaxCalculator : IFederalTaxCalculator
{
    private readonly IValidator<FederalTaxPerson> taxPersonValidator;
    private readonly IFederalTaxRateRepository federalTaxRateRepository;

    public ProprietaryFederalTaxCalculator(
        IValidator<FederalTaxPerson> taxPersonValidator,
        IFederalTaxRateRepository federalTaxRateRepository)
    {
        this.taxPersonValidator = taxPersonValidator;
        this.federalTaxRateRepository = federalTaxRateRepository;
    }

    /// <inheritdoc/>
    public Task<Either<string, BasisTaxResult>> CalculateAsync(int calculationYear, FederalTaxPerson person)
    {
        Option<ValidationResult> validationResult = taxPersonValidator.Validate(person);

        return validationResult
            .Where(r => !r.IsValid)
            .Map<Either<string, bool>>(r =>
            {
                var errorMessageLine = string.Join(";", r.Errors.Select(x => x.ErrorMessage));
                return $"validation failed: {errorMessageLine}";
            })
            .IfNone(true)
            .Bind(_ => Map(person.CivilStatus))

            // get all income level candidate
            .Map(typeId => federalTaxRateRepository
                .TaxRates(calculationYear, typeId)
                .ToList()
                .Where(item => item.IncomeLevel <= person.TaxableAmount)
                .OrderByDescending(item => item.IncomeLevel)
                .DefaultIfEmpty(new FederalTaxTariffModel())
                .First())

            // calculate result
            .Map(tariff => CalculateTax(person, tariff))
            .AsTask();
    }

    private BasisTaxResult CalculateTax(FederalTaxPerson person, FederalTaxTariffModel tariff)
    {
        var referenceTaxableIncome =
            person.TaxableAmount - person.TaxableAmount % tariff.IncomeIncrement;

        var incrementMultiplier = (referenceTaxableIncome - tariff.IncomeLevel) / tariff.IncomeIncrement;

        var baseTaxAmount = incrementMultiplier * tariff.TaxIncrement + tariff.TaxAmount;

        return new BasisTaxResult
        {
            DeterminingFactorTaxableAmount = referenceTaxableIncome,
            TaxAmount = baseTaxAmount,
        };
    }

    private Either<string, TariffType> Map(Option<CivilStatus> status)
    {
        return status.Match<Either<string, TariffType>>(
            Some: s => s switch
            {
                CivilStatus.Single => TariffType.Base,
                CivilStatus.Married => TariffType.Married,
                _ => TariffType.Undefined
            },
            None: () => "Civil status unknown");
    }
}
