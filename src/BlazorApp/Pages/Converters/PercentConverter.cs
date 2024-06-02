using MudBlazor;

namespace BlazorApp.Pages.Converters;

public static class PercentConverter
{
    public static Converter<decimal?> NullableDecimalConverter = new()
    {
        SetFunc = value => $"{value:P2}",
        GetFunc = text =>
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            return decimal.TryParse(text.Replace("%", "").TrimEnd(), out var result)
                ? result / 100
                : null;
        },
    };

    public static Converter<decimal> DecimalConverter = new()
    {
        SetFunc = value => $"{value:P2}",
        GetFunc = text =>
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            return decimal.TryParse(text.Replace("%", "").TrimEnd(), out var result)
                ? result / 100
                : 0;
        },
    };
}
