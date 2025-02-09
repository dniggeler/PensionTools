using Domain.Models.Cashflows;

namespace CashFlowCalculationEngine.Server.Domain;

public record AccountTransactionResponse
{
    public IEnumerable<AccountTransactionResult> ExogenousAccounts { get; set; } = [];

    public IEnumerable<AccountTransactionResult> IncomeAccounts { get; set; } = [];

    public IEnumerable<AccountTransactionResult> WealthAccounts { get; set; } = [];

    public IEnumerable<AccountTransactionResult> InvestmentAccounts { get; set; } = [];

    public IEnumerable<AccountTransactionResult> OccupationalPensionAccounts { get; set; } = [];

    public IEnumerable<AccountTransactionResult> ThirdPillarAccounts { get; set; } = [];
}
