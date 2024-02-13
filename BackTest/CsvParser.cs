namespace BackTest
{
    internal static class CsvParser
    {
        internal record struct Row(string Data);
        internal record struct Cells(string DataCol0, string DataCol2);

        internal static CompanyData Parse(CompanyName name, IEnumerable<Row> csvData) =>
            new(name, csvData.GetData());

        private static IReadOnlyDictionary<DateTime, PriceAtTime> GetData(this IEnumerable<Row> csvData) =>
            csvData.RemoveHeader().
            GetFirstColumns().
            ParseData().
            RemoveNulls().
            DistinctBy(d => d.Date).
            ToDictionary();

        // TODO: Move to common library at some point
        private static IEnumerable<T> RemoveNulls<T>(this IEnumerable<T?> source) where T : struct =>
            source.Where(r => r.HasValue).Select(r => r!.Value);

        private static IEnumerable<Cells> GetFirstColumns(this IEnumerable<Row> csvData) =>
            csvData.Select(s => new Cells(s.Data.Split(',').First(), s.Data.Split(',').ElementAt(2)));

        private static IEnumerable<(DateTime Date, PriceAtTime Price)?> ParseData(this IEnumerable<Cells> source) =>
            source.Select(c =>
            {
                var dateSuccess = DateTime.TryParse(c.DataCol0, out var res);
                DateTime? dateTime = dateSuccess ? res : null;
                
                var priceSuccess = double.TryParse(c.DataCol2, out var price);
                double? priceDouble = priceSuccess ? price : null;
                
                (DateTime, PriceAtTime)? priceAtTime = 
                priceSuccess && dateSuccess ? 
                new(dateTime!.Value, new PriceAtTime(price)) : 
                null;

                return priceAtTime;
            });

        private static IEnumerable<Row> RemoveHeader(this IEnumerable<Row> csvData) =>
             csvData.Skip(1);
    }
}
