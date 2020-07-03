using System;
using System.Collections.Generic;
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

namespace TaxCalculator
{
    public class PollTaxCalculator : IPollTaxCalculator
    {
        private static readonly Dictionary<Canton, decimal> AllCantonsWithPollTax = new Dictionary<Canton, decimal>
        {
            { Canton.ZH, 24M },
            { Canton.LU, 24M },
            { Canton.SO, 30M },
        };

        private readonly IValidator<PollTaxPerson> personValidator;
        private readonly Func<TaxRateDbContext> dbContextFunc;

        public PollTaxCalculator(
            IValidator<PollTaxPerson> personValidator, Func<TaxRateDbContext> dbContextFunc)
        {
            this.personValidator = personValidator;
            this.dbContextFunc = dbContextFunc;
        }

        public Task<Either<string, PollTaxResult>> CalculateAsync(
            int calculationYear, int municipalityId, Canton canton, PollTaxPerson person)
        {
            if (!this.HasPollTax(canton))
            {
                return Task.FromResult<Either<string, PollTaxResult>>(new PollTaxResult());
            }

            var validationResult = this.personValidator.Validate(person);
            if (!validationResult.IsValid)
            {
                var errorMessageLine = string.Join(";", validationResult.Errors
                    .Select(x => x.ErrorMessage));
                return Task
                    .FromResult<Either<string, PollTaxResult>>(
                        $"validation failed: {errorMessageLine}");
            }

            using (var dbContext = this.dbContextFunc())
            {
                Option<TaxRateEntity> taxRate = dbContext.Rates.AsNoTracking()
                    .FirstOrDefault(item => item.BfsId == municipalityId
                                            && item.Year == calculationYear);

                return (from status in person.CivilStatus
                        from nbrOfPolls in this.GetNumberOfPolls(status)
                        from rate in taxRate
                        select new PollTaxResult
                        {
                            CantonTaxAmount = nbrOfPolls * AllCantonsWithPollTax[canton],
                            MunicipalityTaxAmount = nbrOfPolls * rate.PollTaxAmount.IfNone(0),
                        })
                    .ToEither("No tax available")
                    .AsTask();
            }
        }

        public Task<Either<string, PollTaxResult>> CalculateAsync(
            int calculationYear, Canton canton, PollTaxPerson person, TaxRateEntity taxRateEntity)
        {
            if (!this.HasPollTax(canton))
            {
                return Task.FromResult<Either<string, PollTaxResult>>(new PollTaxResult());
            }

            var validationResult = this.personValidator.Validate(person);
            if (!validationResult.IsValid)
            {
                var errorMessageLine = string.Join(";", validationResult.Errors
                    .Select(x => x.ErrorMessage));
                return Task
                    .FromResult<Either<string, PollTaxResult>>(
                        $"validation failed: {errorMessageLine}");
            }

            return (from status in person.CivilStatus
                    from nbrOfPolls in this.GetNumberOfPolls(status)
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
            if (!this.HasPollTax(canton))
            {
                return new PollTaxResult();
            }

            var validationResult = this.personValidator.Validate(person);
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
}