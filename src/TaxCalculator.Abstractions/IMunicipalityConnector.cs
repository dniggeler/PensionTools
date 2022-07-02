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
        /// <param name="limit"></param>
        IAsyncEnumerable<ZipModel> GetAllZipCodesAsync(int limit);

        Task<int> PopulateWithZipCodeAsync();

        /// <summary>
        /// Populate municipality data with ESTV's tax location id.
        /// If doClear is true, tax location id is set to null first.
        /// </summary>
        /// <returns>Number of cases processed.</returns>
        Task<int> PopulateWithTaxLocationAsync(bool doClear);

        /// <summary>
        /// Stage table with zip codes from Post OpenData API.
        /// </summary>
        /// <returns>Number of cases processed.</returns>
        Task<int> StagePlzTableAsync();

        /// <summary>
        /// Clean municipality name. E.g. removing canton postfix to avoid ambiguity.
        /// </summary>
        /// <returns>Number of cases processed.</returns>
        Task<int> CleanMunicipalityName();
    }
}
