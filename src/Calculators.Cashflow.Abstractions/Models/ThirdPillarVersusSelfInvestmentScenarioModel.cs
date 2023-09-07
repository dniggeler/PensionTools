namespace Calculators.CashFlow.Models;

public class ThirdPillarVersusSelfInvestmentScenarioModel
{
    /// <summary>
    /// Final transfer-in year. At this year the third-pillar account is closed and the money is transferred to the wealth.
    /// The capital withdrawn from the third pillar account is taxed by the capital benefits tax.
    /// </summary>
    public int FinalYear { get; set; }

    /// <summary>
    /// The amount invested in the third pillar or self-investment account.
    /// First investment is done in the calculation year up to final year.
    /// </summary>
    public decimal InvestmentAmount { get; set; }

    /// <summary>
    /// Gets or sets yearly excess return on the investment account compared to third pillar account.
    /// There must be an excess return, otherwise the third pillar account is always better because of the tax deduction.
    /// </summary>
    public decimal InvestmentExcessReturn { get; set; }
}
