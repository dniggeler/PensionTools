using System.Collections.Generic;
using ApexCharts;
using static BlazorApp.Pages.SimpleTax.TaxCurve;

namespace BlazorApp.Services;

public class ApexChartConfigurator : IApexChartConfigurator
{
    const string PrimaryColorLight = "#594ae2ff";
    const string PrimaryColorDark = "#776be7ff";

    const string SecondaryColorLight = "#00c853ff";
    const string SecondaryColorDark = "#0bba83ff";

    public ApexChartOptions<CurvePoint> CurvePointOptions(bool isDarkMode)
    {
        var options = new ApexChartOptions<CurvePoint>
        {
            Chart = new Chart
            {
                Toolbar = new Toolbar
                {
                    Show = true,
                    Tools = new Tools
                    {
                        Download = true,
                        Selection = false,
                        Zoom = false,
                        Zoomin = false,
                        Zoomout = false,
                        Pan = false,
                        Reset = false,
                    },
                },
            },
            Stroke =
                new Stroke
                {
                    Curve = Curve.Stepline,
                    Colors = new List<string> { PrimaryColor(isDarkMode), SecondaryColor(isDarkMode) },
                    Width = 2
                },
            Grid = new Grid { Show = true, Xaxis = new GridXAxis { Lines = new Lines { Show = true } }, },
            Xaxis = new XAxis
            {
                Labels = new XAxisLabels
                {
                    Formatter = "function(val) { return val.toFixed(0) }",
                    Style = new AxisLabelStyle { Colors = new ApexCharts.Color(AxisColor(isDarkMode)) }
                },
            },
            Yaxis = new List<YAxis>
            {
                new()
                {
                    Labels = new YAxisLabels
                    {
                        Style = new AxisLabelStyle { Colors = new Color(AxisColor(isDarkMode)) }
                    },
                    TickAmount = 5,
                    DecimalsInFloat = 2,
                }
            }
        };

        return options;
    }

    public string PrimaryColor(bool isDarkMode) => isDarkMode ? PrimaryColorDark : PrimaryColorLight;

    public string SecondaryColor(bool isDarkMode) => isDarkMode ? SecondaryColorDark : SecondaryColorLight;

    public string AxisColor(bool isDarkMode) => isDarkMode ? "lightgrey" : "darkgrey";
}
