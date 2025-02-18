namespace Domain.Models.AccountInputs;

public class AccountInput
{
    public IncomeAccountInput[] IncomeAccounts { get; set; } = [];
    
    public IEnumerable<InvestmentAccountInput> InvestmentAccounts { get; set; } = [];
    
    public IEnumerable<WealthAccountInput> WealthAccounts { get; set; } = [];
    
    public IEnumerable<OccupationalPensionAccountInput> OccupationalPensionAccounts { get; set; } = [];
    
    public ThirdPillarAccountInput[] ThirdPillarAccounts { get; set; } = [];

    public LiabilityAccountInput[] LiabilityAccounts { get; set; } = [];

    public ExogenousAccountInput[] ExogenousAccounts { get; set; } = [];
}
