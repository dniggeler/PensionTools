using System.Collections.Generic;
using System.Threading.Tasks;
using Calculators.CashFlow.Models;

namespace Calculators.CashFlow
{
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
        /// <param name="person"></param>
        /// <param name="cashFlowDefinitionHolder"></param>
        /// <returns></returns>
        Task<MultiPeriodCalculationResult> CalculateAsync(
            MultiPeriodCalculatorPerson person,
            CashFlowDefinitionHolder cashFlowDefinitionHolder);


        /// <summary>
        /// Calculates the asynchronous.
        /// </summary>
        /// <param name="startingYear">The starting year.</param>
        /// <param name="numberOfPeriods">The number of periods.</param>
        /// <param name="person">The person.</param>
        /// <param name="cashFlowDefinitionHolder">The cash flow definition holder.</param>
        /// <returns></returns>
        Task<MultiPeriodCalculationResult> CalculateAsync(
            int startingYear,
            int numberOfPeriods,
            MultiPeriodCalculatorPerson person,
            CashFlowDefinitionHolder cashFlowDefinitionHolder);
    }
}
