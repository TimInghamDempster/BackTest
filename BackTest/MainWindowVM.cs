using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace BackTest
{
    internal class MainWindowVM
    {
        public PlotModel MainPlot { get; }

        public MainWindowVM(IMarketData marketData, IEnumerable<IPriceSeries> series)
        {
            MainPlot = new PlotModel() { Title = "Results" };

            var test = new DummyPriceSeries();
            var start = DateTimeAxis.ToDouble(marketData.FirstEntryDate);
            var end = DateTimeAxis.ToDouble(marketData.LastEntryDate);
            var step = 0.3f;


            MainPlot.Axes.Add(new DateTimeAxis()
            {
                Position = AxisPosition.Bottom,
                Title = "Time",
                Minimum = start,
                Maximum = end
            });

            MainPlot.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, Title = "Value ($)" });

            foreach (var s in series)
            {
                MainPlot.Series.Add(new FunctionSeries(t => s.Price(DateTimeAxis.ToDateTime(t)).Price, start, end, step, "cos(x)"));
            }
        }
    }
}
