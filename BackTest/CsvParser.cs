namespace BackTest
{
    internal static class CsvParser
    {
        // Arguably too complex for a single linq statement, think
        // about breaking this down into smaller methods
        internal static CompanyData Parse(IEnumerable<string> csvData) =>
            new (
                csvData.Skip(1).
                Select(s => s.Split(',').First()).
                Select(d =>
                {
                    var success = DateTime.TryParse(d, out var res);
                    DateTime? dateTime = success ? res : null;
                    return dateTime;
                }).
                Where(r => r.HasValue).
                Select(r => r!.Value).ToList());
    }
}
