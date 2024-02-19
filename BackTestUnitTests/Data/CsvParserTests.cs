using BackTest.Data;
using FluentAssertions;
using static BackTest.Data.CsvParser;

namespace BackTestUnitTests.Data
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
            var companyData = Parse(new(""), new[]
            {
                new Row("Header"),
                new Row("1990-1-12,0,0,0,0,0,0,0"),
                new Row("2000-2-3,0,0,0,0,0,0,0"),
                new Row("2010-4-6,0,0,0,0,0,0,0")
            });

            // Assert
            companyData.Data.First().Key.Should().Be(startDate);
            companyData.Data.Last().Key.Should().Be(endDate);
            companyData.Data.ElementAt(1).Key.Should().Be(midDate);
        }

        [Test]
        public void ExtractsPrices()
        {
            // Arrange
            var startDate = new DateTime(1990, 1, 12);
            var midDate = new DateTime(2000, 2, 3);
            var endDate = new DateTime(2010, 4, 6);


            // Act
            var companyData = Parse(new(""), new[]
            {
                new Row("Header"),
                new Row("1990-1-12,0,3,0,0,0,0,0"),
                new Row("2000-2-3,0,40,0,0,0,0"),
                new Row("2010-4-6,0,2.5,0,0,0,0,0")
            });

            // Assert
            companyData.Data.First().Value.Should().Be(new PriceAtTime(3));
            companyData.Data.Last().Value.Should().Be(new PriceAtTime(2.5));
            companyData.Data.ElementAt(1).Value.Should().Be(new PriceAtTime(40));
        }
    }
}