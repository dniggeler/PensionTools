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
    /// Gets or sets the net growth rate for the investment. The investment capital is fully taxed by the wealth tax.
    /// </summary>
    public decimal InvestmentNetGrowthRate { get; set; }
    
    /// <summary>
    /// Gets or sets the net income yield for the investment. Income types are dividends and interests which are subject to income tax.
    /// </summary>
    public decimal InvestmentNetIncomeYield { get; set; }

    /// <summary>
    /// Gets or sets the net growth rate for the third pillar account.
    /// </summary>
    public decimal ThirdPillarNetGrowthRate { get; set; }
}
