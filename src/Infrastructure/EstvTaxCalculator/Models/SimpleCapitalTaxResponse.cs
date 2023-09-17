using System.Text.Json.Serialization;
using Infrastructure.EstvTaxCalculator.Client.Models;

namespace Infrastructure.EstvTaxCalculator.Models;

public class SimpleCapitalTaxResponse
{
    [JsonPropertyName("response")]
    public SimpleCapitalTaxResult[] Response { get; set; }
}
