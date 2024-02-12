using BackTest;
using FluentAssertions;

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
            var companyData = CsvParser.Parse(new[]
            {
                "1990-1-12,0,0,0,0,0,0,0",
                "2000-2-3,0,0,0,0,0,0,0",
                "2010-4-6,0,0,0,0,0,0,0"
            });

            // Assert
            companyData.Dates.First().Should().Be(startDate);
            companyData.Dates.Last().Should().Be(endDate);
            companyData.Dates.ElementAt(1).Should().Be(midDate);
        }
    }
}