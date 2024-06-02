using Domain.Models.Municipality;

namespace Infrastructure.DataStaging
{
    public interface IDataStagingConnector
    {
        /// <summary>
        /// Copy zip code data from stage table to data store.
        /// </summary>
        /// <returns></returns>
        Task<int> PopulateWithZipCodeAsync();

        /// <summary>
        /// Populate data store containing tax data with ESTV tax location id.
        /// This is one one time step which may repeated every time a municipality change is available.
        /// </summary>
        /// <param name="doClear"></param>
        /// <returns></returns>
        Task<int> PopulateWithTaxLocationAsync(bool doClear);

        /// <summary>
        /// Stage table with zip codes from Post OpenData API.
        /// </summary>
        /// <returns>Number of cases processed.</returns>
        Task<int> StagePlzTableAsync();

        /// <summary>
        /// Get all current zip (PLZ) information for Switzerland supplied by the Swiss Post.
        /// </summary>
        /// <param name="limit"></param>
        IAsyncEnumerable<ZipModel> GetAllZipCodesAsync(int limit);

        /// <summary>
        /// Clean municipality name. E.g. removing canton postfix to avoid ambiguity.
        /// </summary>
        /// <returns>Number of cases processed.</returns>
        Task<int> CleanMunicipalityName();
    }
}
