using System.Collections.Generic;
using System.Threading.Tasks;
using PensionCoach.Tools.PostOpenApi.Models;

namespace PensionCoach.Tools.TaxCalculator.Abstractions;

public interface IAdminConnector
{
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
}
