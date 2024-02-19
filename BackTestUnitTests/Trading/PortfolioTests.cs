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

        // TODO: Can't implement this until we have a 
        // way to price a portfolio
        [Test]
        public void BuyingDoesntAffectBalance()
        {
            throw new NotImplementedException();
        }

        // TODO: Can't implement this until we have a 
        // way to price a portfolio
        [Test]
        public void SellingDoesntAffectBalance()
        {
            throw new NotImplementedException();
        }
    }
}
