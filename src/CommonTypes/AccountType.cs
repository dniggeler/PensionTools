using System.Text.Json.Serialization;

namespace PensionCoach.Tools.CommonTypes
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AccountType
    {
        Exogenous,
        Wealth,
        Income,

        /// <summary>
        /// Capital benefit assets like pension plan funds or 3a accounts which
        /// are subject to capital benefits tax scheme.
        /// </summary>
        CapitalBenefits
    }
}
