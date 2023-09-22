using System.Text.Json.Serialization;

namespace Infrastructure.PostOpenApi.Models
{
    public class OpenApiZipInfo
    {
        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }

        [JsonPropertyName("records")]
        public IEnumerable<OpenApiZipRecord> Records { get; set; }
    }
}
