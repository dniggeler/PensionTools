using System.Text.Json.Serialization;
using Infrastructure.EstvTaxCalculator.Client.Models;

namespace Infrastructure.EstvTaxCalculator.Models;

public class TaxLocationResponse
{
    [JsonPropertyName("response")]
    public TaxLocation[] Response { get; set; }
}
