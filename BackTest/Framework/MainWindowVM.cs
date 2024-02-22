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

        public MainWindowVM(MarketAtTime marketAtTime, IMarketData marketData, IEnumerable<IPriceSeries> series)
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

            foreach (var s in series)
            {
                var dataSeries = new LineSeries
                {
                    Title = s.Name,
                };

                MainPlot.Series.Add(dataSeries);

                var date = marketData.FirstEntryDate;

                while (date < marketData.LastEntryDate)
                {
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        date = date.AddDays(1);
                        continue;
                    }

                    if(s is Trader trader)
                    {
                        trader.Update(date);
                    }

                    marketAtTime.SetDate(date);
                    var value = s.Price(date);
                    dataSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(date), value.Price));
                    date = date.AddDays(1);
                }
            }
        }
    }
}
