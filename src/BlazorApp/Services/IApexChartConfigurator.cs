using ApexCharts;
using BlazorApp.Pages.SimpleTax;

namespace BlazorApp.Services;

public interface IApexChartConfigurator
{
    public ApexChartOptions<TaxCurve.CurvePoint> CurvePointOptions(bool isDarkMode);
    
    public string PrimaryColor(bool isDarkMode);

    public string SecondaryColor(bool isDarkMode);

    public string AxisColor(bool isDarkMode);
}
