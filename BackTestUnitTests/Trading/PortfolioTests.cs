using BackTest.Data;
using BackTest.Trading;
using FluentAssertions;
using NSubstitute;

namespace BackTestUnitTests.Trading
{
    public class PortfolioTests
    {
        [Test]
        public void CantBuyWithoutCash()
        {
            // Arrange
            var portfolio = new Portfolio(new(), new List<Stock>());
            var market = Substitute.For<IMarketAtTime>();
            market.GetPriceAtTime(Arg.Any<CompanyName>(), Arg.Any<DateTime>())
                .Returns(new PriceAtTime(1.0));
            market.Companies.Returns(new List<CompanyName> { new("Company A") });

            // Act
            var result = portfolio.Execute(new Trade.Buy(new("Company A"), 1), market);

            // Assert
            result.IsFaulted.Should().BeTrue();
        }

        [Test]
        public void CanBuyWithCash()
        {
            // Arrange
            var portfolio = new Portfolio(new(1.0), new List<Stock>());
            var market = Substitute.For<IMarketAtTime>();
            market.GetPriceAtTime(Arg.Any<CompanyName>(), Arg.Any<DateTime>())
                .Returns(new PriceAtTime(1.0));
            market.Companies.Returns(new List<CompanyName> { new("Company A") });
            
            // Act
            var result = portfolio.Execute(new Trade.Buy(new("Company A"), 1), market);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.IfSucc(result => result.Cash.Amount.Should().Be(0.0));
            result.IfSucc(result => result.Stocks.Should().Contain(new Stock(new("Company A"), 1)));
        }

        [Test]
        public void CantSellWithoutStock()
        {
            // Arrange
            var portfolio = new Portfolio(new(), new List<Stock>());
            var market = Substitute.For<IMarketAtTime>();
            market.GetPriceAtTime(Arg.Any<CompanyName>(), Arg.Any<DateTime>())
                .Returns(new PriceAtTime(1.0));
            
            // Act
            var result = portfolio.Execute(new Trade.Sell(new("Company A"), 1), market);

            // Assert
            result.IsFaulted.Should().BeTrue();
        }

        [Test]
        public void CanSellWithStock()
        {
            // Arrange
            var portfolio = new Portfolio(new(), new List<Stock> { new(new("Company A"), 1) });
            var market = Substitute.For<IMarketAtTime>();
            market.GetPriceAtTime(Arg.Any<CompanyName>(), Arg.Any<DateTime>())
                .Returns(new PriceAtTime(1.0));
            
            // Act
            var result = portfolio.Execute(new Trade.Sell(new("Company A"), 1), market);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.IfSucc(result => result.Cash.Amount.Should().Be(1.0));
            result.IfSucc(result => result.Stocks.Should().BeEmpty());
        }

        [Test]
        public void CanPartialSellWithStock()
        {
            // Arrange
            var portfolio = new Portfolio(new(), new List<Stock> { new(new("Company A"), 2) });
            var market = Substitute.For<IMarketAtTime>();
            market.GetPriceAtTime(Arg.Any<CompanyName>(), Arg.Any<DateTime>())
                .Returns(new PriceAtTime(1.0));

            // Act
            var result = portfolio.Execute(new Trade.Sell(new("Company A"), 1), market);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.IfSucc(result => result.Cash.Amount.Should().Be(1.0));
            result.IfSucc(result => result.Stocks.Should().Contain(new Stock(new("Company A"), 1)));
        }

        [Test]
        public void CantBuyCompanyThatDoesntExist()
        {
            // Arrange
            var portfolio = new Portfolio(new(), new List<Stock>());
            var market = Substitute.For<IMarketAtTime>();
            market.Companies.Returns(Enumerable.Empty<CompanyName>());
            
            // Act
            var result = portfolio.Execute(new Trade.Buy(new("Company A"), 1), market);

            // Assert
            result.IsFaulted.Should().BeTrue();
        }

        [Test]
        public void BuyingDoesntAffectBalance()
        { 
            // Arrange
            var portfolio = new Portfolio(new(1), new List<Stock>());
            var market = Substitute.For<IMarketAtTime>();
            market.GetPriceAtTime(Arg.Any<CompanyName>(), Arg.Any<DateTime>())
                .Returns(new PriceAtTime(1.0));
            market.Companies.Returns(new List<CompanyName> { new("Company A") });
            var valueBefore = portfolio.Evaluate(market, new DateTime(2020, 1, 1));

            // Act
            var result = portfolio.Execute(new Trade.Buy(new("Company A"), 1), market);

            // Assert
            result.IfSucc(p => p.Evaluate(market, new DateTime(2020, 1, 1))
                .Should().Be(valueBefore));
        }

        [Test]
        public void SellingDoesntAffectBalance()
        { 
            // Arrange
            var portfolio = new Portfolio(new(), new List<Stock> { new(new("Company A"), 1) });
            var market = Substitute.For<IMarketAtTime>();
            market.GetPriceAtTime(Arg.Any<CompanyName>(), Arg.Any<DateTime>())
                .Returns(new PriceAtTime(1.0));
            var valueBefore = portfolio.Evaluate(market, new DateTime(2020, 1,1));

            // Act
            var result = portfolio.Execute(new Trade.Sell(new("Company A"), 1), market);

            // Assert
            result.IfSucc(p => p.Evaluate(market, new DateTime(2020, 1,1))
                .Should().Be(valueBefore));
        }
    }
}
