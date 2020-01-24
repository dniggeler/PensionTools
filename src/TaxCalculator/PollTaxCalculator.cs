using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;
using Tax.Data.Abstractions.Models;


namespace TaxCalculator
{
    public class PollTaxCalculator : IPollTaxCalculator
    {
        private const decimal PollTaxAmount = 24M;
        private static readonly Canton[] CantonsWithPollTax = { Canton.ZH, Canton.LU, Canton.SO };
        private readonly IValidator<PollTaxPerson> personValidator;

        public PollTaxCalculator(IValidator<PollTaxPerson> personValidator)
        {
            this.personValidator = personValidator;
        }

        public Task<Either<string, Option<decimal>>> CalculateAsync(
            int calculationYear, Canton canton, PollTaxPerson person)
        {
            var validationResult = this.personValidator.Validate(person);
            if (!validationResult.IsValid)
            {
                var errorMessageLine = string.Join(";", validationResult.Errors
                    .Select(x => x.ErrorMessage));
                return Task
                    .FromResult<Either<string, Option<decimal>>>(
                        $"validation failed: {errorMessageLine}");
            }

            if (!this.HasPollTax(canton))
            {
                return Task.FromResult<Either<string, Option<decimal>>>(Option<decimal>.None);
            }

            return (from status in person.CivilStatus
                    from nbrOfPolls in this.GetNumberOfPolls(status)
                    select nbrOfPolls * PollTaxAmount)
                .Match<Either<string, Option<decimal>>>(
                    Some: r => Prelude.Right<Option<decimal>>(r),
                    None: () => "No tax available")
                .AsTask();
        }

        public Task<Either<string, Option<decimal>>> CalculateAsync(int calculationYear, Canton canton, PollTaxPerson person, TaxRateEntity taxRateEntity)
        {
            throw new System.NotImplementedException();
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
            return CantonsWithPollTax.Contains(canton);
        }
    }
}