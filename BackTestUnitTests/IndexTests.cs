using BackTest;
using FluentAssertions;
using NSubstitute;

namespace BackTestUnitTests
{
    public class IndexTests
    {
        [Test]
        public void WholeMarketSumsMarket()
        {
            // Arrange
            var companyCount = 20;
            var companyPrice = 2;

            var market = Substitute.For<IMarketAtTime>();
            market.Companies.Returns(
                Enumerable.Range(0, companyCount).Select(i => new CompanyName(i.ToString())));
            market.GetPriceAtTime(Arg.Any<CompanyName>(), Arg.Any<DateTime>()).
                Returns(new PriceAtTime(companyPrice));

            var index = BackTest.Index.WholeMarket(market);

            // Act
            var price = index.Price(new DateTime(2021, 1, 1));

            // Assert
            price.Price.Should().Be(companyCount * companyPrice);
        }

        [Test]
        public void TakeSumsFirstCompanies()
        {
            // Arrange
            var companyCount = 20;
            var companyPrice = 2;

            var market = Substitute.For<IMarketAtTime>();
            market.Companies.Returns(
                Enumerable.Range(0, companyCount).Select(i => new CompanyName(i.ToString())));
            market.GetPriceAtTime(Arg.Any<CompanyName>(), Arg.Any<DateTime>()).
                Returns(new PriceAtTime(companyPrice));

            var index = BackTest.Index.Take(market, companyCount / 2);

            // Act
            var price = index.Price(new DateTime(2021, 1, 1));

            // Assert
            price.Price.Should().Be(companyCount / 2 * companyPrice);
        }


        [Test]
        public void TopSumsBestPrices()
        {
            // Arrange
            var companyCount = 20;
            var companyPrice = 2;

            var market = Substitute.For<IMarketAtTime>();
            market.Companies.Returns(
                Enumerable.Range(0, companyCount).Select(i => new CompanyName(i.ToString())));
            
            var price = (string name) => int.Parse(name) % 2 == 0 ? companyPrice : 0;

            market.GetPriceAtTime(Arg.Any<CompanyName>(), Arg.Any<DateTime>()).
                Returns(x => new PriceAtTime(price(((CompanyName)x[0]).Name)));

            var index = BackTest.Index.Top(market, companyCount / 2);

            // Act
            var indexPrice = index.Price(new DateTime(2021, 1, 1));

            // Assert
            indexPrice.Price.Should().Be(companyCount / 2 * companyPrice);
        }
    }
}
