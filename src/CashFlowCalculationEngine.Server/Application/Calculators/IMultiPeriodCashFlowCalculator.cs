using CashFlowCalculationEngine.Server.Domain.Calculator;
using CashFlowCalculationEngine.Server.Domain.CashFlows;
using CashFlowCalculationEngine.Server.Domain.Graph.Actions;
using CashFlowCalculationEngine.Server.Domain.Location;
using CashFlowCalculationEngine.Server.Domain.Person;
using Domain.Models.AccountInputs;

namespace CashFlowCalculationEngine.Server.Application.Calculators;

public interface IMultiPeriodCashFlowCalculator
{
    /// <summary>
    /// Calculates how taxable assets evolves over time.
    /// Takes a list of cash-flow definitions sums them up by groups of target/source pairs along the timeline.
    /// Then, iterates along the timeline and calculates for a given year:
    /// 1. adds cash-flow amount for the given year to its associated asset type
    /// 2. calculates tax amount for each target asset type
    /// 3. deduct tax amount from asset values
    /// 4. move flow asset types to its stock asset type (ie. salary does not stay after paying tax for it but
    ///    is moved to taxable wealth).
    /// </summary>
    /// <param name="calculationParameters"></param>
    /// <param name="person"></param>
    /// <param name="municipality"></param>
    /// <param name="accountHolder"></param>
    /// <param name="cashFlowHolder"></param>
    /// <param name="taxationActionHolder"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<MultiPeriodCalculationResponse> CalculateAsync(
        CalculationParameters calculationParameters,
        CalculationPerson person,
        Municipality municipality,
        AccountInput accountHolder,
        CashFlowInput cashFlowHolder,
        TaxActionInput taxationActionHolder,
        CancellationToken cancellationToken);
}
