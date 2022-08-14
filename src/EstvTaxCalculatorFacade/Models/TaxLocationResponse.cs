using System.Text.Json.Serialization;
using PensionCoach.Tools.EstvTaxCalculators.Abstractions.Models;

namespace PensionCoach.Tools.EstvTaxCalculators.Models;

public class TaxLocationResponse
{
    [JsonPropertyName("response")]
    public TaxLocation[] Response { get; set; }
}
