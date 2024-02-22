using BackTest.Data;
using BackTest.Framework;
using BackTest.Trading;
using FluentAssertions;
using NSubstitute;

namespace BackTestUnitTests.Trading
{
    public class TraderTests
    {
        [Test]
        public void EvaluatesPortfolio()
        {
            // Arrange
            var portfolio = new Portfolio(new(1.0), new List<Stock>());
            var market = Substitute.For<IMarketAtTime>();
            market.GetPriceAtTime(Arg.Any<CompanyName>(), Arg.Any<DateTime>())
                .Returns(new PriceAtTime(1.0));
            market.Companies.Returns(new List<CompanyName> { new("Company A") });
            var trader = new Trader(portfolio, new("Test"), market, Substitute.For<IStrategy>());
            
            // Act
            var result = trader.Price(new DateTime(1,1,1));
            
            // Assert
            result.Price.Should().Be(1.0);
        }

        [Test]
        public void ExecutesStrategy()
        {
            // Arrange
            var portfolio = new Portfolio(new(1.0), new List<Stock>());
            var market = Substitute.For<IMarketAtTime>();
            var strategy = Substitute.For<IStrategy>();
            strategy.GenerateOrder(market, Arg.Any<DateTime>(), portfolio)
                .Returns(new Order(new List<Trade>()));
            var trader = new Trader(portfolio, new("Test"), market, strategy);
            var date = new DateTime(1,1,1);

            // Act
            trader.Update(date);

            // Assert
            strategy.Received().GenerateOrder(market, date, portfolio);
        }
    }
}
