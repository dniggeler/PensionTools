using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Municipality;
using PensionCoach.Tools.CommonTypes.Tax;

namespace BlazorApp.Services;

public record MunicipalityFilter
{
    public int[] BfsNumberList { get; set; } = Array.Empty<int>();

    public Canton[] CantonList { get; set; }
}

public interface IMunicipalityService
{
        
    Task<IEnumerable<MunicipalityModel>> GetAllAsync();

    Task<IEnumerable<TaxSupportedMunicipalityModel>> GetTaxSupportingAsync();

    Task<IEnumerable<TaxSupportedMunicipalityModel>> GetTaxSupportingAsync(MunicipalityFilter filter);
}
