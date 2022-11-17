using System.Text.Json.Serialization;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions.Models;

namespace PensionCoach.Tools.EstvTaxCalculators.Models;

public class SimpleCapitalTaxResponse
{
    [JsonPropertyName("response")]
    public SimpleCapitalTaxResult[] Response { get; set; }
}
