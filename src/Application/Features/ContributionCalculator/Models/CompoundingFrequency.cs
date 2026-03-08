using System.Text.Json.Serialization;

namespace Application.Features.ContributionCalculator.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CompoundingFrequency
{
    Daily,
    Monthly,
    Quarterly,
    SemiAnnual,
    Annual,
    Continuous
}
