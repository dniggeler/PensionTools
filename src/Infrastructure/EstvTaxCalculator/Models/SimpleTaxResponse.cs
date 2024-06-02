using System.Text.Json.Serialization;
using Application.Tax.Estv.Client.Models;

namespace Infrastructure.EstvTaxCalculator.Models
{
    public class SimpleTaxResponse
    {
        [JsonPropertyName("response")]
        public SimpleTaxResult Response { get; set; }
    }
}
