namespace BackTest
{
    internal interface IMarketData
    {
        public DateTime FirstEntryDate { get; }
        public DateTime LastEntryDate { get; }
        IEnumerable<CompanyName> Companies { get; }
    }

    internal class MarketData : IMarketData
    {
        private IReadOnlyDictionary<CompanyName, CompanyData> _data;

        public MarketData(IDataSource dataSource)
        {
            var companies = dataSource.GetCompanies();

            Companies = companies.Keys.ToList();

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

        internal PriceAtTime GetPriceAtTime(CompanyName name, DateTime startDate) =>
            _data[name].Data[startDate];
    }

    internal record struct CompanyName(string Name);

    internal record struct PriceAtTime(double Price);

    internal record CompanyData(CompanyName Name, IReadOnlyDictionary<DateTime, PriceAtTime> Data);
}
