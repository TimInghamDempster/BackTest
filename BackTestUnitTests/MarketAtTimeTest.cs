using BackTest;
using FluentAssertions;
using NSubstitute;

namespace BackTestUnitTests
{
    public class MarketAtTimeTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetsPriceInPast()
        {
            // Arrange
            var marketData = Substitute.For<IMarketData>();
            marketData.GetPriceAtTime(Arg.Any<CompanyName>(), Arg.Any<DateTime>())
                .Returns(new PriceAtTime(0.0));
            var marketAtTime = new MarketAtTime(marketData);
            marketAtTime.SetDate(new DateTime(2000, 1, 1));

            // Act
            var price = marketAtTime.GetPriceAtTime(new("Company A"), new DateTime(1990,1,1));

            // Assert
            price.Price.Should().Be(0.0);
        }

        [Test]
        public void FailsInFuture()
        {
            // Arrange
            var marketData = Substitute.For<IMarketData>();
            marketData.GetPriceAtTime(Arg.Any<CompanyName>(), Arg.Any<DateTime>())
                .Returns(new PriceAtTime(0.0));
            var marketAtTime = new MarketAtTime(marketData);
            marketAtTime.SetDate(new DateTime(2000, 1, 1));

            // Act / Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => marketAtTime.GetPriceAtTime(
                new("Company A"), new DateTime(2010, 1, 1)));
        }
    }
}