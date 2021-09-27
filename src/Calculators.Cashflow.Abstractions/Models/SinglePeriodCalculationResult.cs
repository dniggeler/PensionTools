﻿using PensionCoach.Tools.CommonTypes;

namespace Calculators.CashFlow.Models
{
    public record SinglePeriodCalculationResult(int Year, decimal Amount, AccountType AccountType);
}
