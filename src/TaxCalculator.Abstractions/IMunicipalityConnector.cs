using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Municipality;

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
