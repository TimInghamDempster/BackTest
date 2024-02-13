namespace BackTest
{
    internal static class CsvParser
    {
        internal record struct Row(string Data);
        internal record struct Cell(string Data);

        internal static CompanyData Parse(IEnumerable<Row> csvData) =>
            new(csvData.GetDates());

        private static IEnumerable<DateTime> GetDates(this IEnumerable<Row> csvData) =>
            csvData.RemoveHeader().
            GetFirstColumns().
            ParseDates().
            RemoveNulls();

        // TODO: Move to common library at some point
        private static IEnumerable<T> RemoveNulls<T>(this IEnumerable<T?> source) where T : struct =>
            source.Where(r => r.HasValue).Select(r => r!.Value);

        private static IEnumerable<Cell> GetFirstColumns(this IEnumerable<Row> csvData) =>
            csvData.Select(s => new Cell(s.Data.Split(',').First()));

        private static IEnumerable<DateTime?> ParseDates(this IEnumerable<Cell> source) =>
            source.Select(c =>
            {
                var success = DateTime.TryParse(c.Data, out var res);
                DateTime? dateTime = success ? res : null;
                return dateTime;
            });

        private static IEnumerable<Row> RemoveHeader(this IEnumerable<Row> csvData) =>
             csvData.Skip(1);
    }
}
