using System.Text.Json.Serialization;

namespace PensionCoach.Tools.PostOpenApi.Models;

public class OpenApiZipInfo
{
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    [JsonPropertyName("records")]
    public IEnumerable<OpenApiZipRecord> Records { get; set; }
}
