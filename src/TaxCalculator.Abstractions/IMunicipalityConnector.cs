using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models.Municipality;


namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface IMunicipalityConnector
    {
        Task<IEnumerable<MunicipalityModel>> GetAllAsync();

        IEnumerable<MunicipalityModel> Search(MunicipalitySearchFilter searchFilter);

        Task<Either<string, MunicipalityModel>> GetAsync(int bfsNumber);
    }
}