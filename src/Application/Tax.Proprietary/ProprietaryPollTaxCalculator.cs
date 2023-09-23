using Application.Tax.Proprietary.Abstractions.Models;
using Application.Tax.Proprietary.Abstractions.Models.Person;
using Application.Tax.Proprietary.Abstractions.Repositories;
using Application.Tax.Proprietary.Contracts;
using Domain.Enums;
using Domain.Models.Tax;
using FluentValidation;
using LanguageExt;

namespace Application.Tax.Proprietary;

public class ProprietaryPollTaxCalculator : IPollTaxCalculator
{
    private static readonly Dictionary<Canton, decimal> AllCantonsWithPollTax = new()
    {
        { Canton.ZH, 24M },
        { Canton.LU, 24M },
        { Canton.SO, 30M },
    };

    private readonly IValidator<PollTaxPerson> personValidator;
    private readonly IStateTaxRateRepository stateTaxRateRepository;

    public ProprietaryPollTaxCalculator(
        IValidator<PollTaxPerson> personValidator,
        IStateTaxRateRepository stateTaxRateRepository)
    {
        this.personValidator = personValidator;
        this.stateTaxRateRepository = stateTaxRateRepository;
    }

    public Task<Either<string, PollTaxResult>> CalculateAsync(
        int calculationYear, int municipalityId, Canton canton, PollTaxPerson person)
    {
        if (!HasPollTax(canton))
        {
            return Task.FromResult<Either<string, PollTaxResult>>(new PollTaxResult());
        }

        var validationResult = personValidator.Validate(person);
        if (!validationResult.IsValid)
        {
            var errorMessageLine = string.Join(";", validationResult.Errors
                .Select(x => x.ErrorMessage));
            return Task
                .FromResult<Either<string, PollTaxResult>>(
                    $"validation failed: {errorMessageLine}");
        }

        Option<TaxRateEntity> taxRate = stateTaxRateRepository.TaxRates(calculationYear, municipalityId);

        return (from nbrOfPolls in GetNumberOfPolls(person.CivilStatus)
                from rate in taxRate
                select new PollTaxResult
                {
                    CantonTaxAmount = nbrOfPolls * AllCantonsWithPollTax[canton],
                    MunicipalityTaxAmount = nbrOfPolls * rate.PollTaxAmount.IfNone(0),
                })
            .ToEither("No tax available")
            .AsTask();
    }

    public Task<Either<string, PollTaxResult>> CalculateAsync(
        int calculationYear, Canton canton, PollTaxPerson person, TaxRateEntity taxRateEntity)
    {
        if (!HasPollTax(canton))
        {
            return Task.FromResult<Either<string, PollTaxResult>>(new PollTaxResult());
        }

        var validationResult = personValidator.Validate(person);
        if (!validationResult.IsValid)
        {
            var errorMessageLine = string.Join(";", validationResult.Errors
                .Select(x => x.ErrorMessage));
            return Task
                .FromResult<Either<string, PollTaxResult>>(
                    $"validation failed: {errorMessageLine}");
        }

        return (from nbrOfPolls in GetNumberOfPolls(person.CivilStatus)
                select new PollTaxResult
                {
                    CantonTaxAmount = nbrOfPolls * AllCantonsWithPollTax[canton],
                    MunicipalityTaxAmount = nbrOfPolls * taxRateEntity.PollTaxAmount.IfNone(0M),
                })
            .ToEither("No tax available")
            .AsTask();
    }

    public Either<string, PollTaxResult> CalculateInternal(
        int calculationYear, Canton canton, PollTaxPerson person)
    {
        if (!HasPollTax(canton))
        {
            return new PollTaxResult();
        }

        var validationResult = personValidator.Validate(person);
        if (!validationResult.IsValid)
        {
            var errMsg = string
                .Join(";", validationResult.Errors
                    .Select(x => x.ErrorMessage));

            return $"validation failed: {errMsg}";
        }

        return "none";
    }

    private Option<int> GetNumberOfPolls(CivilStatus status)
    {
        return status switch
        {
            CivilStatus.Married => 2,
            CivilStatus.Single => 1,
            _ => Option<int>.None
        };
    }

    private bool HasPollTax(Canton canton)
    {
        return AllCantonsWithPollTax.ContainsKey(canton);
    }
}
