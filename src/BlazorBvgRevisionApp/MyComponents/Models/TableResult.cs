public record TableResult(string Benefit, decimal ValueCurrentBvg, decimal? ValueBvgRevision)
{
    public decimal? Difference => ValueBvgRevision.HasValue ? Math.Round(ValueBvgRevision.Value - ValueCurrentBvg) : null;
}
