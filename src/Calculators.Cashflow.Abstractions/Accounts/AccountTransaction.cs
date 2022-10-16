using System;

namespace Calculators.CashFlow.Accounts;

public record AccountTransaction(string Description, DateTime ValutaDate, decimal Amount);
