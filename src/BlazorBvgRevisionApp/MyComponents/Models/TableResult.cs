public record TableResult(string Benefit, decimal? ValueCurrentBvg, decimal? ValueBvgRevision, bool IsSubordinated = false)
{
    public decimal? Difference
    {
        get
        {
            decimal? nullableDifference = ValueBvgRevision - ValueCurrentBvg;

            return nullableDifference.HasValue ? Math.Round(nullableDifference.Value) : null;
        }
    }
}
