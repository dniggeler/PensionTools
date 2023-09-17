using Domain.Contracts;

namespace Domain.Models.MultiPeriod.Definitions;

public record SetupAccountDefinition : ICompositeCashFlowDefinition
{
    /// <summary>
    /// Gets or sets the initial wealth.
    /// </summary>
    /// <value>
    /// The initial wealth.
    /// </value>
    public decimal InitialWealth { get; set; }

    /// <summary>
    /// Gets or sets the initial occupational pension assets (Berufliche Vorsorge).
    /// </summary>
    public decimal InitialOccupationalPensionAssets { get; set; }

    /// <summary>
    /// Gets or sets the initial third pillar assets (3A Konto).
    /// </summary>
    public decimal InitialThirdPillarAssets { get; set; }

    /// <summary>
    /// Gets or sets the initial investment assets. Investment assets have two
    /// cash flow streams:
    /// 1. Capital growth
    /// 2. Interest payments (eg. dividends) which are paid out (not accumulated)
    /// </summary>
    public decimal InitialInvestmentAssets { get; set; }
}
