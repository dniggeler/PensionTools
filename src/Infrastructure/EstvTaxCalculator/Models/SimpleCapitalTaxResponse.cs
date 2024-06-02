using System.Text.Json.Serialization;
using Application.Tax.Estv.Client.Models;

namespace Infrastructure.EstvTaxCalculator.Models
{
    public class SimpleCapitalTaxResponse
    {
        [JsonPropertyName("response")]
        public SimpleCapitalTaxResult[] Response { get; set; }
    }
}
