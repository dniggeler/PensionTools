using Domain.Models.MultiPeriod;
using LanguageExt;
using PensionCoach.Tools.CommonTypes.MultiPeriod;

namespace Application.MultiPeriodCalculator
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
        /// <param name="startingYear"></param>
        /// <param name="minimumNumberOfPeriods"></param>
        /// <param name="person"></param>
        /// <param name="cashFlowDefinitionHolder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<Either<string, MultiPeriodCalculationResult>> CalculateAsync(
            int startingYear,
            int minimumNumberOfPeriods,
            MultiPeriodCalculatorPerson person,
            CashFlowDefinitionHolder cashFlowDefinitionHolder,
            MultiPeriodOptions options);

    }
}
