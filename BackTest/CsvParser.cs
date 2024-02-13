namespace BackTest
{
    internal static class CsvParser
    {
        internal record struct Row(string Data);
        internal record struct Cells(string DataCol0, string DataCol2);

        internal static CompanyData Parse(IEnumerable<Row> csvData) =>
            new(csvData.GetData());

        private static IEnumerable<PriceAtTime> GetData(this IEnumerable<Row> csvData) =>
            csvData.RemoveHeader().
            GetFirstColumns().
            ParseData().
            RemoveNulls();

        // TODO: Move to common library at some point
        private static IEnumerable<T> RemoveNulls<T>(this IEnumerable<T?> source) where T : class =>
            source.Where(r => r is not null).Select(r => r!);

        private static IEnumerable<Cells> GetFirstColumns(this IEnumerable<Row> csvData) =>
            csvData.Select(s => new Cells(s.Data.Split(',').First(), s.Data.Split(',').ElementAt(2)));

        private static IEnumerable<PriceAtTime?> ParseData(this IEnumerable<Cells> source) =>
            source.Select(c =>
            {
                var dateSuccess = DateTime.TryParse(c.DataCol0, out var res);
                DateTime? dateTime = dateSuccess ? res : null;
                
                var priceSuccess = double.TryParse(c.DataCol2, out var price);
                double? priceDouble = priceSuccess ? price : null;

                return priceSuccess && dateSuccess ? new PriceAtTime(dateTime!.Value, price) : null;
            });

        private static IEnumerable<Row> RemoveHeader(this IEnumerable<Row> csvData) =>
             csvData.Skip(1);
    }
}
