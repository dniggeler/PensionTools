using Application.Municipality;
using Application.Tax.Estv.Client;
using Application.Tax.Estv.Client.Models;
using AutoMapper;
using Domain.Enums;
using Domain.Models.Municipality;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Tax.Estv
{
    public class EstvMunicipalityServiceClient(
        IMapper mapper,
        IEstvTaxCalculatorClient estvTaxCalculatorClient) : IMunicipalityConnector
    {
        public async Task<IEnumerable<MunicipalityModel>> GetAllAsync()
        {
            TaxLocation[] taxLocations = await estvTaxCalculatorClient.GetTaxLocationsAsync(string.Empty, string.Empty);

            return taxLocations.Select(location => new MunicipalityModel
            {
                EstvTaxLocationId = location.Id, BfsNumber = location.BfsId, Canton = Enum.Parse<Canton>(location.Canton), Name = location.BfsName
            });
        }

        /// <summary>
        /// Searches the specified search filter.
        /// </summary>
        /// <param name="searchFilter">The search filter.</param>
        /// <returns>List of municipalities.</returns>
        public async  Task<IEnumerable<MunicipalityModel>> SearchAsync(MunicipalitySearchFilter searchFilter)
        {
            TaxLocation[] taxLocations = await estvTaxCalculatorClient.GetTaxLocationsAsync(string.Empty, searchFilter.Name);

            return taxLocations.Select(location => new MunicipalityModel
            {
                EstvTaxLocationId = location.Id,
                BfsNumber = location.BfsId,
                Canton = Enum.Parse<Canton>(location.Canton),
                Name = location.BfsName
            });
        }

        public async Task<Either<string, MunicipalityModel>> GetAsync(int bfsNumber, int year)
        {
            TaxLocation[] taxLocations = await estvTaxCalculatorClient.GetTaxLocationsAsync(string.Empty, string.Empty);

            return taxLocations
                .Select(location => new MunicipalityModel
                {
                    EstvTaxLocationId = location.Id, BfsNumber = location.BfsId, Canton = Enum.Parse<Canton>(location.Canton), Name = location.BfsName
                })
                .FirstOrDefault();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<TaxSupportedMunicipalityModel>> GetAllSupportTaxCalculationAsync()
        {
            const int maxEstvSupportedYear = 2022;

            TaxLocation[] taxLocations = await estvTaxCalculatorClient.GetTaxLocationsAsync(string.Empty, string.Empty);

            IReadOnlyCollection<TaxSupportedMunicipalityModel> list = taxLocations
                .Select(item => new TaxSupportedMunicipalityModel
                {
                    BfsMunicipalityNumber = item.BfsId,
                    Name = item.BfsName,
                    Canton = Enum.Parse<Canton>(item.Canton),
                    MaxSupportedYear = maxEstvSupportedYear,
                    EstvTaxLocationId = item.Id
                })
                .ToList();

            return list;
        }
    }
}
