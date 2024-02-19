namespace BackTest.Data
{
    internal interface IMarketData
    {
        DateTime FirstEntryDate { get; }
        DateTime LastEntryDate { get; }
        IEnumerable<CompanyName> Companies { get; }
        PriceAtTime GetPriceAtTime(CompanyName name, DateTime date);
    }

    internal class MarketData : IMarketData
    {
        private IReadOnlyDictionary<CompanyName, CompanyData> _data;

        public MarketData(IDataSource dataSource)
        {
            var companies = dataSource.GetCompanies();

            // Very crude data sanitization, there is a lot of bad data
            // in the data set, with impossibly high prices so assume
            // anything over 5k is bad data.  We will miss some genuine
            // companies but this is a toy project.
            Companies = companies.
                Where(c => c.Value.Data.Max(d => d.Value.Price) < 5000).
                Select(kvp => kvp.Key).
                ToList();

            _data = dataSource.GetCompanies();

            var dates = companies.
                SelectMany(c => c.Value.Data).
                Select(d => d.Key).
                Order().
                ToList();

            FirstEntryDate = dates.First();
            LastEntryDate = dates.Last();
        }

        public DateTime FirstEntryDate { get; private init; }

        public DateTime LastEntryDate { get; private init; }

        public IEnumerable<CompanyName> Companies { get; private init; }

        /// <summary>
        /// return 0 if no data is found for the company at the given date,
        /// the assumption is that any shares owned at that time would be
        /// worthless.
        /// </summary>
        public PriceAtTime GetPriceAtTime(CompanyName name, DateTime date) =>
            _data[name].Data.ContainsKey(date) ?
            _data[name].Data[date] : FillData(date, name);

        private PriceAtTime FillData(DateTime date, CompanyName name)
        {
            for (int i = 0; i < 7; i++)
            {
                if (_data[name].Data.ContainsKey(date.AddDays(-i)))
                {
                    return _data[name].Data[date.AddDays(-i)];
                }
                else if (_data[name].Data.ContainsKey(date.AddDays(i)))
                {
                    return _data[name].Data[date.AddDays(i)];
                }
            }
            return new(0);
        }
    }

    internal record struct CompanyName(string Name);

    internal record struct PriceAtTime(double Price);

    internal record CompanyData(CompanyName Name, IReadOnlyDictionary<DateTime, PriceAtTime> Data);
}
