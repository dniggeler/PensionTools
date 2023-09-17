using System.Text.Json.Serialization;

namespace Infrastructure.PostOpenApi.Models;

public class OpenApiZipDetail
{
    [JsonPropertyName("timestamp")]
    public DateTime TimeStamp { get; set; }

    [JsonPropertyName("fields")]
    public OpenApiZipFields Fields { get; set; }
}
