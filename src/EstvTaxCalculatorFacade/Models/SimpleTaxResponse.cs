using System.Text.Json.Serialization;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions.Models;

namespace PensionCoach.Tools.EstvTaxCalculators.Models;

public class SimpleTaxResponse
{
    [JsonPropertyName("response")]
    public SimpleTaxResult Response { get; set; }
}
