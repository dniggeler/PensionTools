using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using Tax.Tools.Comparison.Abstractions;
using System.Threading.Channels;
using System.Threading;
using Application.Municipality;
using Application.Tax.Proprietary.Abstractions;
using Domain.Models.Municipality;
using Domain.Models.Tax;
using PensionCoach.Tools.TaxComparison;

namespace Tax.Tools.Comparison
{
    public class TaxComparer : ITaxComparer
    {
        private readonly IValidator<CapitalBenefitTaxPerson> taxPersonValidator;
        private readonly IValidator<TaxPerson> fullTaxPersonValidator;
        private readonly IFullCapitalBenefitTaxCalculator capitalBenefitCalculator;
        private readonly IFullWealthAndIncomeTaxCalculator fullWealthAndIncomeCalculator;
        private readonly IMunicipalityConnector municipalityConnector;

        public TaxComparer(
            IValidator<CapitalBenefitTaxPerson> taxPersonValidator,
            IValidator<TaxPerson> fullTaxPersonValidator,
            IFullCapitalBenefitTaxCalculator capitalBenefitCalculator,
            IFullWealthAndIncomeTaxCalculator fullWealthAndIncomeCalculator,
            IMunicipalityConnector municipalityConnector)
        {
            this.taxPersonValidator = taxPersonValidator;
            this.fullTaxPersonValidator = fullTaxPersonValidator;
            this.capitalBenefitCalculator = capitalBenefitCalculator;
            this.fullWealthAndIncomeCalculator = fullWealthAndIncomeCalculator;
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
                .Select(_ => CalculateCapitalBenefitsTaxAsync(bufferChannel, resultChannel, person))
                .ToArray();

            int totalCount = 0;
            foreach (TaxSupportedMunicipalityModel municipality in municipalities)
            {
                totalCount++;
                await bufferChannel.Writer.WriteAsync(municipality);
            }

            bufferChannel.Writer.Complete();

            await Task.WhenAll(readers);

            resultChannel.Writer.Complete();

            await foreach (Either<string, CapitalBenefitTaxComparerResult> result in resultChannel.Reader.ReadAllAsync(CancellationToken.None))
            {
                result.Iter(r => r.TotalCount = totalCount);
                yield return result;
            }
        }

        public async IAsyncEnumerable<Either<string, IncomeAndWealthTaxComparerResult>> CompareIncomeAndWealthTaxAsync(TaxPerson person, int[] bfsNumbers)
        {
            const int numberOfWorkers = 20;

            var validationResult = await fullTaxPersonValidator.ValidateAsync(person);

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
            var resultChannel = Channel.CreateUnbounded<Either<string, IncomeAndWealthTaxComparerResult>>();

            Task[] readers = Enumerable
                .Range(1, numberOfWorkers)
                .Select(_ => CalculateIncomeAndWealthTaxAsync(bufferChannel, resultChannel, person))
                .ToArray();

            int totalCount = 0;
            foreach (TaxSupportedMunicipalityModel municipality in municipalities)
            {
                totalCount++;
                await bufferChannel.Writer.WriteAsync(municipality);
            }

            bufferChannel.Writer.Complete();

            await Task.WhenAll(readers);

            resultChannel.Writer.Complete();

            await foreach (Either<string, IncomeAndWealthTaxComparerResult> result in resultChannel.Reader.ReadAllAsync(CancellationToken.None))
            {
                result.Iter(r => r.TotalCount = totalCount);

                yield return result;
            }
        }

        private async Task CalculateCapitalBenefitsTaxAsync(
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

        private async Task CalculateIncomeAndWealthTaxAsync(
            Channel<TaxSupportedMunicipalityModel> bufferChannel,
            Channel<Either<string, IncomeAndWealthTaxComparerResult>> resultChannel,
            TaxPerson person)
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

                Either<string, IncomeAndWealthTaxComparerResult> result = await fullWealthAndIncomeCalculator
                    .CalculateAsync(data.MaxSupportedYear, municipalityModel, person)
                    .MapAsync(r => new IncomeAndWealthTaxComparerResult
                    {
                        MunicipalityId = municipalityModel.BfsNumber,
                        MunicipalityName = municipalityModel.Name,
                        Canton = municipalityModel.Canton,
                        MaxSupportedTaxYear = data.MaxSupportedYear,
                        TaxResult = r
                    });

                await resultChannel.Writer.WriteAsync(result);
            }
        }
    }
}
