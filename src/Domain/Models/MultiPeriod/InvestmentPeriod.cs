namespace Domain.Models.MultiPeriod
{
    public record InvestmentPeriod
    {
        public int Year { get; set; }
        public int NumberOfPeriods { get; set; }
    }
}
