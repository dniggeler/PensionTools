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
        /// A investment account which is subject to wealth tax scheme.
        /// However, if it pays dividends, they are subjects to the income tax.
        /// </summary>
        /// <remarks>
        /// Planned feature: its value may change stochastically.
        /// </remarks>
        Investment,

        /// <summary>
        /// Capital benefit assets in second pillar account (BVG Vorsorgegelder, FZP) which
        /// are subject to capital benefits tax scheme.
        /// </summary>
        OccupationalPension,

        /// <summary>
        /// Capital benefit assets in third pillar account (3a) which
        /// are subject to capital benefits tax scheme. 
        /// </summary>
        ThirdPillar,

        /// <summary>
        /// Accumulated tax assets which are subject to all tax types like income, wealth and capital benefits tax scheme.
        /// </summary>
        Tax
    }
}
