using System.Text.Json.Serialization;

namespace Infrastructure.PostOpenApi.Models;

public class OpenApiZipRecord
{
    [JsonPropertyName("record")]
    public OpenApiZipDetail Record { get; set; }
}
