using System.Text.Json.Serialization;
using Infrastructure.EstvTaxCalculator.Client.Models;

namespace Infrastructure.EstvTaxCalculator.Models;

public class SimpleTaxResponse
{
    [JsonPropertyName("response")]
    public SimpleTaxResult Response { get; set; }
}
