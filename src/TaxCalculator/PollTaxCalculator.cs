using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Person;


namespace TaxCalculator
{
    public class PollTaxCalculator : IPollTaxCalculator
    {
        private readonly IValidator<PollTaxPerson> _personValidator;
        private const decimal PollTaxAmount = 24M;

        public PollTaxCalculator(IValidator<PollTaxPerson> personValidator)
        {
            _personValidator = personValidator;
        }

        public Task<Either<string, decimal>> CalculateAsync(int calculationYear,
            PollTaxPerson person)
        {
            var validationResult = _personValidator.Validate(person);
            if (!validationResult.IsValid)
            {
                var errorMessageLine = string.Join(";", validationResult.Errors
                    .Select(x => x.ErrorMessage));
                return Task
                    .FromResult<Either<string,decimal>>( $"validation failed: {errorMessageLine}");
            }

            return person.CivilStatus
                .Map(GetNumberOfPolls)
                .Match<Either<string, decimal>>(Some: v => v * PollTaxAmount,
                    None: () => "No tax available")
                .AsTask();
        }

        private int GetNumberOfPolls(CivilStatus status)
        {
            return status switch
            {
                CivilStatus.Married => 2,
                CivilStatus.Single => 1,
                _ => 0
            };
        }
    }
}