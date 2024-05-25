using BackTest.Data;
using BackTest.Trading;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;

namespace BackTest.Framework
{
    internal class MainWindowVM
    {
        public PlotModel MainPlot { get; }

        public MainWindowVM(MarketAtTime marketAtTime, IMarketData marketData, IEnumerable<IPriceSeriesCollection> seriesCollections)
        {
            MainPlot = new PlotModel() { Title = "Results" };

            var start = DateTimeAxis.ToDouble(marketData.FirstEntryDate);
            var end = DateTimeAxis.ToDouble(marketData.LastEntryDate);

            MainPlot.Axes.Add(new DateTimeAxis()
            {
                Position = AxisPosition.Bottom,
                Title = "Time",
                Minimum = start,
                Maximum = end
            });

            MainPlot.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, Title = "Value ($)" });

            MainPlot.Legends.Add(new Legend()
            {
                LegendPlacement = LegendPlacement.Inside,
                LegendPosition = LegendPosition.TopLeft
            });

            foreach (var c in seriesCollections)
            {
                var series = new Dictionary<string, LineSeries>();

                foreach (var s in c.Series)
                {
                    var dataSeries = new LineSeries
                    {
                        Title = s.Name,
                    };

                    MainPlot.Series.Add(dataSeries);
                    series.Add(s.Name, dataSeries);
                }

                var date = marketData.FirstEntryDate;

                while (date < marketData.LastEntryDate)
                {
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        date = date.AddDays(1);
                        continue;
                    }

                    marketAtTime.SetDate(date);

                    if (c is Trader trader)
                    {
                        trader.Update(date);
                    }

                    foreach (var source in c.Series)
                    {
                        var dataSeries = series[source.Name];
                        var value = source.Price(date);
                        dataSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(date), value.Price));
                    }
                
                    date = date.AddDays(1);
                }
            }
        }
    }
}
