using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.CommonTypes.Tax;

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
        /// <param name="year">The year.</param>
        /// <returns></returns>
        Task<IReadOnlyCollection<TaxSupportedMunicipalityModel>> GetAllSupportTaxCalculationAsync(int year);
    }
}
