namespace MultiPeriodGraphClient;

public class BalanceSheet
{
    public record BalanceSheetEntry
    {
        public decimal? TotalWealth { get; set; }

        public decimal? TotalLiability { get; set; }

        public decimal? TotalThirdPillar { get; set; }

        public decimal? TotalOccupationalPension { get; set; }

        public decimal? Total => (TotalWealth.HasValue || TotalThirdPillar.HasValue || TotalOccupationalPension.HasValue)
            ? (TotalWealth ?? 0) + (TotalThirdPillar ?? 0) + (TotalOccupationalPension ?? 0) - (TotalLiability ?? 0)
            : null;
    }

    public BalanceSheetEntry? Entry { get; set; }
}
