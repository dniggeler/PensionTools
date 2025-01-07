using System.Text.Json.Serialization;

namespace Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AccountType
    {
        /// <summary>
        /// An account which is not subject to any tax scheme. It represents the outside world of the domain.
        /// No behavior is defined. But each transaction needs two linked accounts.
        /// For example, salary comes from outside and goes to the income account.
        /// </summary>
        Exogenous,
        Wealth,
        Income,

        /// <summary>
        /// An investment account which is subject to wealth tax scheme.
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
        /// Accumulated tax amounts which are subject to all tax types like income, wealth and capital benefits tax scheme.
        /// It is cleared at the end of the year.
        /// </summary>
        Tax
    }
}
