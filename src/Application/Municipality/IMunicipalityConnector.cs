using Domain.Models.Municipality;
using Domain.Models.Tax;
using LanguageExt;

namespace Application.Municipality
{
    public interface IMunicipalityConnector
    {
        Task<IEnumerable<MunicipalityModel>> GetAllAsync();

        IEnumerable<MunicipalityModel> Search(MunicipalitySearchFilter searchFilter);

        Task<Either<string, MunicipalityModel>> GetAsync(int bfsNumber, int year);

        /// <summary>
        /// Gets all municipalities supporting tax calculation.
        /// Municipalities are sorted by their name.
        /// </summary>
        Task<IReadOnlyCollection<TaxSupportedMunicipalityModel>> GetAllSupportTaxCalculationAsync();
    }
}
