using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace BackTest
{
    internal class MainWindowVM
    {
        public PlotModel MainPlot { get; }

        public MainWindowVM(IMarketData marketData)
        {
            MainPlot = new PlotModel() { Title = "Results" };

            MainPlot.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
            
            MainPlot.Axes.Add(new DateTimeAxis() { 
                Position = AxisPosition.Bottom, 
                Title = "Time",
                Minimum = DateTimeAxis.ToDouble(marketData.FirstEntryDate),
                Maximum = DateTimeAxis.ToDouble(marketData.LastEntryDate)}
            );
            
            MainPlot.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, Title = "Value ($)" });
        }
    }
}
