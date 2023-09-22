namespace Infrastructure.DataStaging;

public interface ITaxDataPopulateService
{
    /// <summary>
    /// Populate municipality data with ESTV's tax location id.
    /// If doClear is true, tax location id is set to null first.
    /// </summary>
    /// <returns>Number of cases processed.</returns>
    Task<int> PopulateWithTaxLocationAsync(bool doClear);
}
