using BackTest;
using FluentAssertions;
using NSubstitute;

namespace BackTestUnitTests
{
    public class MarketDataTests
    {
        [Test]
        public void GetsCorrectPeriod()
        {
            // Arrange
            var startDate = new DateTime(1990, 1, 12);
            var midDate = new DateTime(2000, 2, 3);
            var endDate = new DateTime(2010, 4, 6);

            var companyA = new CompanyData(new("Company A"),
                new Dictionary<DateTime, PriceAtTime>()
                    { { startDate, new(0.5) }, { midDate, new(0.75) } });
            var companyB = new CompanyData(new("Company B"), 
                new Dictionary<DateTime, PriceAtTime>()
                    { { midDate, new(1.9) }, { endDate, new(5.7) } });

            var companies = new Dictionary<CompanyName, CompanyData>()
                { { companyA.Name, companyA }, { companyB.Name, companyB } };

            var dataSource = Substitute.For<IDataSource>();
            dataSource.GetCompanies().Returns(companies);

            // Act
            var marketData = new MarketData(dataSource);

            // Assert
            marketData.FirstEntryDate.Should().Be(startDate);
            marketData.LastEntryDate.Should().Be(endDate);
        }

        [Test]
        public void ListsCompanies()
        {
            // Arrange
            var startDate = new DateTime(1990, 1, 12);
            var midDate = new DateTime(2000, 2, 3);
            var endDate = new DateTime(2010, 4, 6);

            var companyA = new CompanyData(new("Company A"),
                new Dictionary<DateTime, PriceAtTime>()
                    { { startDate, new(0.5) }, { midDate, new(0.75) } });
            var companyB = new CompanyData(new("Company B"),
                new Dictionary<DateTime, PriceAtTime>()
                    { { midDate, new(1.9) }, { endDate, new(5.7) } });

            var companies = new Dictionary<CompanyName, CompanyData>()
                { { companyA.Name, companyA }, { companyB.Name, companyB } };

            var dataSource = Substitute.For<IDataSource>();
            dataSource.GetCompanies().Returns(companies);

            // Act
            var marketData = new MarketData(dataSource);

            // Assert
            marketData.Companies.Should().Contain(companyA.Name);
            marketData.Companies.Should().Contain(companyB.Name);
        }

        [Test]
        public void GetsPrices()
        {
            // Arrange
            var startDate = new DateTime(1990, 1, 12);
            var midDate = new DateTime(2000, 2, 3);
            var endDate = new DateTime(2010, 4, 6);

            var companyA = new CompanyData(new("Company A"),
                new Dictionary<DateTime, PriceAtTime>()
                    { { startDate, new(0.5) }, { midDate, new(0.75) } });
            var companyB = new CompanyData(new("Company B"),
                new Dictionary<DateTime, PriceAtTime>()
                    { { midDate, new(1.9) }, { endDate, new(5.7) } });

            var companies = new Dictionary<CompanyName, CompanyData>()
                { { companyA.Name, companyA }, { companyB.Name, companyB } };

            var dataSource = Substitute.For<IDataSource>();
            dataSource.GetCompanies().Returns(companies);
            var marketData = new MarketData(dataSource);

            // Act
            var price1 = marketData.GetPriceAtTime(companyA.Name, startDate);
            var price2 = marketData.GetPriceAtTime(companyA.Name, midDate);
            var price3 = marketData.GetPriceAtTime(companyB.Name, midDate);
            var price4 = marketData.GetPriceAtTime(companyB.Name, endDate);

            // Assert
            price1.Price.Should().Be(0.5);
            price2.Price.Should().Be(0.75);
            price3.Price.Should().Be(1.9);
            price4.Price.Should().Be(5.7);
        }

        [Test]
        public void HandlesMissingPrice()
        {
            // Arrange
            var startDate = new DateTime(1990, 1, 12);
            var midDate = new DateTime(2000, 2, 3);
            var endDate = new DateTime(2010, 4, 6);

            var companyA = new CompanyData(new("Company A"),
                new Dictionary<DateTime, PriceAtTime>()
                    { { startDate, new(0.5) }, { midDate, new(0.75) } });

            var companies = new Dictionary<CompanyName, CompanyData>()
                { { companyA.Name, companyA } };

            var dataSource = Substitute.For<IDataSource>();
            dataSource.GetCompanies().Returns(companies);
            var marketData = new MarketData(dataSource);

            // Act
            var price = marketData.GetPriceAtTime(companyA.Name, endDate);

            // Assert
            price.Price.Should().Be(0.0);
        }

        [Test]
        public void FillsShortGaps()
        {
            var startDate = new DateTime(1990, 1, 12);
            var midDate = new DateTime(2000, 2, 3);
            var endDate = new DateTime(2000, 2, 6);

            var companyA = new CompanyData(new("Company A"),
                new Dictionary<DateTime, PriceAtTime>()
                    { { startDate, new(0.5) }, { midDate, new(0.75) } });

            var companies = new Dictionary<CompanyName, CompanyData>()
                { { companyA.Name, companyA } };

            var dataSource = Substitute.For<IDataSource>();
            dataSource.GetCompanies().Returns(companies);
            var marketData = new MarketData(dataSource);

            // Act
            var price = marketData.GetPriceAtTime(companyA.Name, endDate);

            // Assert
            price.Price.Should().Be(0.75);
        }

        [Test]
        public void RemovesOverlyHighPrices()
        {
            var startDate = new DateTime(1990, 1, 12);
            var midDate = new DateTime(2000, 2, 3);
            var endDate = new DateTime(2020, 2, 6);

            var companyA = new CompanyData(new("Company A"),
                new Dictionary<DateTime, PriceAtTime>()
                    { { startDate, new(0.5) }, { midDate, new(7500) } });

            var companies = new Dictionary<CompanyName, CompanyData>()
                { { companyA.Name, companyA } };

            var dataSource = Substitute.For<IDataSource>();
            dataSource.GetCompanies().Returns(companies);

            // Act
            var marketData = new MarketData(dataSource);

            // Assert
            marketData.Companies.Should().BeEmpty();
        }
    }
}