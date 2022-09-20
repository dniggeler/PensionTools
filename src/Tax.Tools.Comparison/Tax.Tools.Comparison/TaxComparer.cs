using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.TaxCalculator.Abstractions;
using Tax.Tools.Comparison.Abstractions;
using Tax.Tools.Comparison.Abstractions.Models;
using System.Threading.Channels;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using System.Threading;
using LanguageExt.UnsafeValueAccess;

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

        public async Task<Either<string, IReadOnlyCollection<CapitalBenefitTaxComparerResult>>> CompareCapitalBenefitTaxAsync(
            CapitalBenefitTaxPerson person)
        {
            var validationResult = taxPersonValidator.Validate(person);
            if (!validationResult.IsValid)
            {
                var errorMessageLine =
                    string.Join(";", validationResult.Errors.Select(x => x.ErrorMessage));
                return $"validation failed: {errorMessageLine}";
            }

            IReadOnlyCollection<TaxSupportedMunicipalityModel> municipalities =
                await municipalityConnector
                    .GetAllSupportTaxCalculationAsync();

            var resultList = new List<CapitalBenefitTaxComparerResult>();

            foreach (TaxSupportedMunicipalityModel municipality in municipalities)
            {
                MunicipalityModel municipalityModel = new MunicipalityModel()
                {
                    Name = municipality.Name,
                    BfsNumber = municipality.BfsMunicipalityNumber,
                    Canton = municipality.Canton,
                    EstvTaxLocationId = municipality.EstvTaxLocationId
                };

                var result =
                    await capitalBenefitCalculator
                        .CalculateAsync(
                            municipality.MaxSupportedYear,
                            municipalityModel,
                            person);

                result
                    .Map(r => new CapitalBenefitTaxComparerResult
                    {
                        MunicipalityId = municipality.BfsMunicipalityNumber,
                        MunicipalityName = municipality.Name,
                        Canton = municipality.Canton,
                        MaxSupportedTaxYear = municipality.MaxSupportedYear,
                        MunicipalityTaxResult = r,
                    })
                    .IfRight(r =>
                        resultList.Add(r));
            }

            return resultList;
        }

        public async IAsyncEnumerable<CapitalBenefitTaxComparerResult> CompareCapitalBenefitTaxAsync(
            CapitalBenefitTaxPerson person, int maxNumberOfMunicipality)
        {
            const int numberOfWorkers = 20;

            var validationResult = await taxPersonValidator.ValidateAsync(person);

            if (!validationResult.IsValid)
            {
                yield break;
            }

            IReadOnlyCollection<TaxSupportedMunicipalityModel> municipalities =
                (await municipalityConnector.GetAllSupportTaxCalculationAsync())
                .Take(maxNumberOfMunicipality)
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
                if (result.IsLeft)
                {
                    continue;
                }

                yield return result.ValueUnsafe();
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

                Either<string, FullCapitalBenefitTaxResult> result = await capitalBenefitCalculator
                    .CalculateAsync(data.MaxSupportedYear, municipalityModel, person);

                if (result.IsLeft)
                {
                    continue;
                }

                var comparerResult = new CapitalBenefitTaxComparerResult
                {
                    MunicipalityId = municipalityModel.BfsNumber,
                    MunicipalityName = municipalityModel.Name,
                    Canton = municipalityModel.Canton,
                    MaxSupportedTaxYear = data.MaxSupportedYear,
                };

                result.Iter(r => comparerResult.MunicipalityTaxResult = r);

                await resultChannel.Writer.WriteAsync(comparerResult);
            }
        }
    }
}
