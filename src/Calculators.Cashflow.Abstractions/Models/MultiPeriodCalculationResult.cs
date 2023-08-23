using System.Collections.Generic;
using Calculators.CashFlow.Accounts;
using PensionCoach.Tools.CommonTypes.MultiPeriod;

namespace Calculators.CashFlow.Models;

public record MultiPeriodCalculationResult
{
    public int StartingYear { get; set; }
        
    public int NumberOfPeriods { get; set; }

    public IEnumerable<SinglePeriodCalculationResult> Accounts{ get; set; }

    public ExogenousAccount ExogenousAccount { get; set; }

    public IncomeAccount IncomeAccount { get; set; }

    public WealthAccount WealthAccount { get; set; }

    public InvestmentAccount InvestmentAccount { get; set; }

    public OccupationalPensionAccount OccupationalPensionAccount { get; set; }

    public ThirdPillarAccount ThirdPillarAccount { get; set; }

    public TaxAccount TaxAccount { get; set; }
}
