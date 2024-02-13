using BackTest;
using FluentAssertions;
using NSubstitute;

namespace BackTestUnitTests
{
    public class MarketDataTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetsCorrectPeriod()
        {
            // Arrange
            var startDate = new DateTime(1990, 1, 12);
            var midDate = new DateTime(2000, 2, 3);
            var endDate = new DateTime(2010, 4, 6);

            var companyA = new CompanyData(new("Company A"), new List<PriceAtTime>() { new(startDate, 0.5), new(midDate, 0.75) });
            var companyB = new CompanyData(new("Company B"), new List<PriceAtTime>() { new(midDate, 1.9), new(endDate, 5.7) });
            var companies = new List<CompanyData>() { companyA, companyB };

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

            var companyA = new CompanyData(new("Company A"), new List<PriceAtTime>() { new(startDate, 0.5), new(midDate, 0.75) });
            var companyB = new CompanyData(new("Company A"), new List<PriceAtTime>() { new(midDate, 1.9), new(endDate, 5.7) });
            var companies = new List<CompanyData>() { companyA, companyB };

            var dataSource = Substitute.For<IDataSource>();
            dataSource.GetCompanies().Returns(companies);

            // Act
            var marketData = new MarketData(dataSource);

            // Assert
            marketData.Companies.Should().Contain(companyA.Name);
            marketData.Companies.Should().Contain(companyB.Name);
        }
    }
}