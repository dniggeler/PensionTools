using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Tax.Data;
using Tax.Data.Abstractions.Models;

namespace PensionCoach.Tools.TaxCalculator.Proprietary;

public class ProprietaryChurchTaxCalculator : IChurchTaxCalculator
{
    private readonly IValidator<ChurchTaxPerson> churchTaxPersonValidator;
    private readonly IValidator<AggregatedBasisTaxResult> taxResultValidator;
    private readonly Func<TaxRateDbContext> taxRateContext;

    public ProprietaryChurchTaxCalculator(
        IValidator<ChurchTaxPerson> churchTaxPersonValidator,
        IValidator<AggregatedBasisTaxResult> basisTaxResultValidator,
        Func<TaxRateDbContext> taxRateContext)
    {
        this.churchTaxPersonValidator = churchTaxPersonValidator;
        taxResultValidator = basisTaxResultValidator;
        this.taxRateContext = taxRateContext;
    }

    /// <inheritdoc />
    public Task<Either<string, ChurchTaxResult>> CalculateAsync(
        int calculationYear,
        int municipalityId,
        ChurchTaxPerson person,
        AggregatedBasisTaxResult taxResult)
    {
        using var ctxt = taxRateContext();
        var taxRateEntity = ctxt.Rates.AsNoTracking()
            .FirstOrDefault(item => item.Year == calculationYear
                                    && item.BfsId == municipalityId);
        if (taxRateEntity == null)
        {
            Either<string, ChurchTaxResult> msg =
                $"No tax rate found for municipality {municipalityId} and year {calculationYear}";

            return msg.AsTask();
        }

        return Validate(person, taxResult)
            .Map(_ => CalculateInternal(person, taxRateEntity, taxResult))
            .AsTask();
    }

    public Task<Either<string, ChurchTaxResult>> CalculateAsync(
        ChurchTaxPerson person, TaxRateEntity taxRateEntity, AggregatedBasisTaxResult taxResult)
    {
        return
            Validate(person, taxResult)
                .Select(_ => CalculateInternal(person, taxRateEntity, taxResult))
                .AsTask();
    }

    private Either<string, bool> Validate(
        ChurchTaxPerson person, AggregatedBasisTaxResult taxResult)
    {
        if (person == null)
        {
            return $"validation failed: {nameof(person)}";
        }

        if (taxResult == null)
        {
            return $"validation failed: {nameof(taxResult)} null";
        }

        var validationResult = churchTaxPersonValidator.Validate(person);

        if (!validationResult.IsValid)
        {
            var errorMessageLine = string.Join(
                ";", validationResult.Errors.Select(x => x.ErrorMessage));

            return $"validation failed: {errorMessageLine}";
        }

        var validationResultForTax = taxResultValidator.Validate(taxResult);

        if (!validationResultForTax.IsValid)
        {
            var errorMessageLine = string.Join(
                ";", validationResultForTax.Errors.Select(x => x.ErrorMessage));

            return $"validation failed: {errorMessageLine}";
        }

        return true;
    }

    private ChurchTaxResult CalculateInternal(
        ChurchTaxPerson person,
        TaxRateEntity taxRateEntity,
        AggregatedBasisTaxResult taxResult)
    {
        var religiousGroupPartner = person.PartnerReligiousGroupType ?? ReligiousGroupType.Other;

        var splitFactor =
            DetermineSplitFactor(person.CivilStatus, person.ReligiousGroupType, person.PartnerReligiousGroupType);

        var ratePerson = GetTaxRate(person.ReligiousGroupType, taxRateEntity);
        var ratePartner = GetTaxRate(religiousGroupPartner, taxRateEntity);

        return new ChurchTaxResult
        {
            TaxAmount = ratePerson / 100M * taxResult.Total * splitFactor,
            TaxAmountPartner = ratePartner / 100M * taxResult.Total * (1M - splitFactor),
            TaxRate = ratePerson,
        };

        static decimal GetTaxRate(ReligiousGroupType religiousGroupType, TaxRateEntity rateEntity)
        {
            return religiousGroupType switch
            {
                ReligiousGroupType.Roman => rateEntity.RomanChurchTaxRate,
                ReligiousGroupType.Protestant => rateEntity.ProtestantChurchTaxRate,
                ReligiousGroupType.Catholic => rateEntity.CatholicChurchTaxRate,
                _ => 0M
            };
        }
    }

    private decimal DetermineSplitFactor(
        CivilStatus civilStatus,
        ReligiousGroupType religiousGroupType,
        ReligiousGroupType? personReligiousGroupPartner)
    {
        var splitFactor = (civilStatus, religiousGroupType, personReligiousGroupPartner)
            switch
        {
            (CivilStatus.Undefined, _, _) => 1.0M,
            (CivilStatus.Single, ReligiousGroupType.Other, _) => 0.0M,
            (CivilStatus.Single, _, _) => 1.0M,
            (CivilStatus.Married, ReligiousGroupType.Other, ReligiousGroupType.Other)
                => 0.0M,
            (CivilStatus.Married, ReligiousGroupType.Other, _) => 1.0M,
            _ => 0.5M
        };

        return splitFactor;
    }
}
