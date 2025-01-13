using Domain.Enums;

namespace Domain.Models.Cashflows.Accounts
{
    public record AccountTransaction(string Description, DateTime ValutaDate, decimal Amount, FlowType Flow = FlowType.Undefined);
}
