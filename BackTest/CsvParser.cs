namespace BackTest
{
    internal static class CsvParser
    {
        internal static CompanyData Parse(IEnumerable<string> csvData)
        {
            var dates = 
                csvData.Select(s => s.Split(',').First()).
                Select(DateTime.Parse);

            return new CompanyData(dates.ToList());
        }
    }
}
