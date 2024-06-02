namespace Domain.Models.Cashflows
{
    public record AccountTransactionResultHolder
    {
        public AccountTransactionResult ExogenousAccount { get; set; }

        public AccountTransactionResult IncomeAccount { get; set; }

        public AccountTransactionResult WealthAccount { get; set; }

        public AccountTransactionResult InvestmentAccount { get; set; }

        public AccountTransactionResult OccupationalPensionAccount { get; set; }

        public AccountTransactionResult ThirdPillarAccount { get; set; }

        public AccountTransactionResult TaxAccount { get; set; }
    }
}
