using BackTest;
using FluentAssertions;
using NSubstitute;
using OxyPlot.Axes;

namespace BackTestUnitTests
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

            // Act
            var vm = new MainWindowVM(marketData, Enumerable.Empty<IPriceSeries>());

            // Assert
            vm.MainPlot.Axes.Should().Contain(
                a =>
                a.Minimum == DateTimeAxis.ToDouble(startDate) &&
                a.Maximum == DateTimeAxis.ToDouble(endDate));
        }
    }
}