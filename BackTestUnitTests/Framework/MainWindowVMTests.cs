using BackTest.Data;
using BackTest.Framework;
using FluentAssertions;
using NSubstitute;
using OxyPlot.Axes;

namespace BackTestUnitTests.Framework
{
    public class MainWindowVMTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void PlotsWholePeriod()
        {
            // Arrange
            var startDate = new DateTime(1990, 1, 12);
            var endDate = new DateTime(2010, 4, 6);
            var marketData = Substitute.For<IMarketData>();
            marketData.FirstEntryDate.Returns(startDate);
            marketData.LastEntryDate.Returns(endDate);
            var marketAtTime = new MarketAtTime(marketData);
            marketAtTime.SetDate(endDate);

            // Act
            var vm = new MainWindowVM(marketAtTime, marketData, Enumerable.Empty<IPriceSeriesCollection>());

            // Assert
            vm.MainPlot.Axes.Should().Contain(
                a =>
                a.Minimum == DateTimeAxis.ToDouble(startDate) &&
                a.Maximum == DateTimeAxis.ToDouble(endDate));
        }

        // TODO: It's really hard to convince the VM to
        // call a mock series, which suggests it needs
        // refactoring.
        [Test]
        public void IgnoresWeekends()
        {
            throw new NotImplementedException();
        }
    }
}