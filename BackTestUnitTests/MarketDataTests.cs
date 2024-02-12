using BackTest;
using FluentAssertions;

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

            var companyA = new CompanyData(new List<DateTime>() { startDate, midDate });
            var companyB = new CompanyData(new List<DateTime>() { midDate, endDate });
            var companies = new List<CompanyData>() { companyA, companyB };

            // Act
            var marketData = new MarketData(companies);

            // Assert
            marketData.FirstEntryDate.Should().Be(startDate);
            marketData.LastEntryDate.Should().Be(endDate);
        }
    }
}