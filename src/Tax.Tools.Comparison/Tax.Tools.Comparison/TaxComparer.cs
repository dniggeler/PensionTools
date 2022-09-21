using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using Tax.Tools.Comparison.Abstractions;
using System.Threading.Channels;
using System.Threading;
using PensionCoach.Tools.TaxComparison;

namespace Tax.Tools.Comparison
{
    public class TaxComparer : ITaxComparer
    {
        private readonly IValidator<CapitalBenefitTaxPerson> taxPersonValidator;
        private readonly IFullCapitalBenefitTaxCalculator capitalBenefitCalculator;
        private readonly IMunicipalityConnector municipalityConnector;

        public TaxComparer(
            IValidator<CapitalBenefitTaxPerson> taxPersonValidator,
            IFullCapitalBenefitTaxCalculator capitalBenefitCalculator,
            IMunicipalityConnector municipalityConnector)
        {
            this.taxPersonValidator = taxPersonValidator;
            this.capitalBenefitCalculator = capitalBenefitCalculator;
            this.municipalityConnector = municipalityConnector;
        }

        public async IAsyncEnumerable<Either<string, CapitalBenefitTaxComparerResult>> CompareCapitalBenefitTaxAsync(
            CapitalBenefitTaxPerson person, int[] bfsNumbers)
        {
            const int numberOfWorkers = 20;

            var validationResult = await taxPersonValidator.ValidateAsync(person);

            if (!validationResult.IsValid)
            {
                var errorMessageLine =
                    string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));
                yield return $"validation failed: {errorMessageLine}";
            }

            // compare all if BFS number list is null or empty
            IReadOnlyCollection<TaxSupportedMunicipalityModel> municipalities =
                (await municipalityConnector.GetAllSupportTaxCalculationAsync())
                .Where(item => bfsNumbers is null || bfsNumbers.Length == 0 || bfsNumbers.Contains(item.BfsMunicipalityNumber))
                .ToList();

            var bufferChannel = Channel.CreateUnbounded<TaxSupportedMunicipalityModel>();
            var resultChannel = Channel.CreateUnbounded<Either<string, CapitalBenefitTaxComparerResult>>();

            Task[] readers = Enumerable
                .Range(1, numberOfWorkers)
                .Select(_ => CalculateAsync(bufferChannel, resultChannel, person))
                .ToArray();

            foreach (TaxSupportedMunicipalityModel municipality in municipalities)
            {
                await bufferChannel.Writer.WriteAsync(municipality);
            }

            bufferChannel.Writer.Complete();

            await Task.WhenAll(readers);

            resultChannel.Writer.Complete();

            await foreach (Either<string, CapitalBenefitTaxComparerResult> result in resultChannel.Reader.ReadAllAsync(CancellationToken.None))
            {
                yield return result;
            }
        }

        private async Task CalculateAsync(
            Channel<TaxSupportedMunicipalityModel> bufferChannel,
            Channel<Either<string, CapitalBenefitTaxComparerResult>> resultChannel,
            CapitalBenefitTaxPerson person)
        {
            while (await bufferChannel.Reader.WaitToReadAsync())
            {
                if (!bufferChannel.Reader.TryRead(out TaxSupportedMunicipalityModel data))
                {
                    continue;
                }

                MunicipalityModel municipalityModel = new MunicipalityModel()
                {
                    Canton = data.Canton,
                    BfsNumber = data.BfsMunicipalityNumber,
                    Name = data.Name,
                    EstvTaxLocationId = data.EstvTaxLocationId,
                };

                Either<string, CapitalBenefitTaxComparerResult> result = await capitalBenefitCalculator
                    .CalculateAsync(data.MaxSupportedYear, municipalityModel, person)
                    .MapAsync(r => new CapitalBenefitTaxComparerResult
                    {
                        MunicipalityId = municipalityModel.BfsNumber,
                        MunicipalityName = municipalityModel.Name,
                        Canton = municipalityModel.Canton,
                        MaxSupportedTaxYear = data.MaxSupportedYear,
                        MunicipalityTaxResult = r
                    });

                await resultChannel.Writer.WriteAsync(result);
            }
        }
    }
}
