namespace BackTest
{
    internal static class CsvParser
    {
        internal static CompanyData Parse(IEnumerable<string> csvData) =>
            new(csvData.GetDates());

        private static IEnumerable<DateTime> GetDates(this IEnumerable<string> csvData) =>
            csvData.RemoveHeader().
            GetFirstColumns().
            ParseDates().
            RemoveNulls();

        // TODO: Move to common library at some point
        private static IEnumerable<T> RemoveNulls<T>(this IEnumerable<T?> source) where T : struct =>
            source.Where(r => r.HasValue).Select(r => r!.Value);

        private static IEnumerable<string> GetFirstColumns(this IEnumerable<string> csvData) =>
            csvData.Select(s => s.Split(',').First());

        private static IEnumerable<DateTime?> ParseDates(this IEnumerable<string> source) =>
            source.Select(d =>
            {
                var success = DateTime.TryParse(d, out var res);
                DateTime? dateTime = success ? res : null;
                return dateTime;
            });

        private static IEnumerable<string> RemoveHeader(this IEnumerable<string> csvData) =>
             csvData.Skip(1);
    }
}
