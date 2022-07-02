using System.Text.Json.Serialization;

namespace PensionCoach.Tools.EstvTaxCalculators.Models;

public class TaxLocationResponse
{
    [JsonPropertyName("response")]
    public TaxLocation[] Response { get; set; }
}
