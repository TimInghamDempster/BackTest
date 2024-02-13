using FluentAssertions;
using static BackTest.CsvParser;

namespace BackTestUnitTests
{
    public class CsvParserTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ExtractsDates()
        {
            // Arrange
            var startDate = new DateTime(1990, 1, 12);
            var midDate = new DateTime(2000, 2, 3);
            var endDate = new DateTime(2010, 4, 6);


            // Act
            var companyData = Parse(new[]
            {
                new Row("Header"),
                new Row("1990-1-12,0,0,0,0,0,0,0"),
                new Row("2000-2-3,0,0,0,0,0,0,0"),
                new Row("2010-4-6,0,0,0,0,0,0,0")
            });

            // Assert
            companyData.Dates.First().Date.Should().Be(startDate);
            companyData.Dates.Last().Date.Should().Be(endDate);
            companyData.Dates.ElementAt(1).Date.Should().Be(midDate);
        }

        [Test]
        public void ExtractsPrices()
        {
            // Arrange
            var startDate = new DateTime(1990, 1, 12);
            var midDate = new DateTime(2000, 2, 3);
            var endDate = new DateTime(2010, 4, 6);


            // Act
            var companyData = Parse(new[]
            {
                new Row("Header"),
                new Row("1990-1-12,0,3,0,0,0,0,0"),
                new Row("2000-2-3,0,40,0,0,0,0"),
                new Row("2010-4-6,0,2.5,0,0,0,0,0")
            });

            // Assert
            companyData.Dates.First().Price.Should().Be(3);
            companyData.Dates.Last().Price.Should().Be(2.5);
            companyData.Dates.ElementAt(1).Price.Should().Be(40);
        }
    }
}