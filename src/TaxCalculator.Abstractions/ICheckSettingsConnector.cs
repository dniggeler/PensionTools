using System.Collections.Generic;
using System.Threading.Tasks;

namespace PensionCoach.Tools.TaxCalculator.Abstractions;

public interface ICheckSettingsConnector
{
    Task<Dictionary<string, string>> GetAsync();
}
