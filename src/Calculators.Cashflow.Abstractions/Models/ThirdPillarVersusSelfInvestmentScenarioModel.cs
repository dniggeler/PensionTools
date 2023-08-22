using System.Collections.Generic;
namespace Calculators.CashFlow.Models;

public class ThirdPillarVersusSelfInvestmentScenarioModel
{
    /// <summary>
    /// Final transfer-in year. At this year the third-pillar account is closed and the money is transferred to the wealth.
    /// </summary>
    public int FinalYear { get; set; }

    /// <summary>
    /// The amount invested in the third pillar or self-investment account.
    /// First investment is done in the calculation year up to final year.
    /// </summary>
    public decimal InvestmentAmount { get; set; }

    /// <summary>
    /// Gets or sets yearly net return on self-investment.
    /// </summary>
    public decimal NetSelfInvestmentReturn { get; set; }

    /// <summary>
    /// Gets or sets yearly net return on third pillar account.
    /// </summary>
    public decimal NetThirdPillarReturn { get; set; }

}
