using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using PensionCoach.Tools.TaxCalculator.Abstractions.Models;

namespace PensionCoach.Tools.TaxCalculator.Abstractions
{
    public interface IMunicipalityConnector
    {
        Task<IEnumerable<MunicipalityModel>> GetAllAsync();

        Task<Either<string, MunicipalityModel>> GetAsync(int bfsNumber);
    }
}