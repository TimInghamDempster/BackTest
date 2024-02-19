using BackTest.Data;
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
            var trader = new Trader(portfolio, new("Test"), market);
            
            // Act
            var result = trader.Price(new DateTime(1,1,1));
            
            // Assert
            result.Price.Should().Be(1.0);
        }
    }
}
