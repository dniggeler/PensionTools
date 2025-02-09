using CashFlowCalculationEngine.Server.Domain;
using CashFlowCalculationEngine.Server.Domain.Graph.Accounts;

namespace CashFlowCalculationEngine.Server.Application.Validators;

internal class ValidatorHelpers
{
    public static List<Guid> AccountIdList(AccountInput model)
    {
        return model.ExogenousAccounts.Select(a => a.Id)
            .Concat(model.IncomeAccounts.Select(a => a.Id))
            .Concat(model.WealthAccounts.Select(a => a.Id))
            .Concat(model.OccupationalPensionAccounts.Select(a => a.Id))
            .Concat(model.ThirdPillarAccounts.Select(a => a.Id))
            .Concat(model.InvestmentAccounts.Select(a => a.Id))
            .ToList();
    }

    public static List<Guid> AccountIdList(AccountTransactionResponse? model)
    {
        if (model is null)
        {
            return [];
        }

        return model.ExogenousAccounts.Select(a => a.Id)
            .Concat(model.IncomeAccounts.Select(a => a.Id))
            .Concat(model.WealthAccounts.Select(a => a.Id))
            .Concat(model.OccupationalPensionAccounts.Select(a => a.Id))
            .Concat(model.ThirdPillarAccounts.Select(a => a.Id))
            .Concat(model.InvestmentAccounts.Select(a => a.Id))
            .ToList();
    }
}
