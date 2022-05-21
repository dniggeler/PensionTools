using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.CommonTypes.Tax;
using PensionCoach.Tools.PostOpenApi.Models;

namespace PensionCoach.Tools.TaxCalculator.Abstractions
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

        /// <summary>
        /// Get all current zip (PLZ) information for Switzerland supplied by the Swiss Post.
        /// </summary>
        Task<IEnumerable<ZipModel>> GetAllZipCodesAsync();
    }
}
