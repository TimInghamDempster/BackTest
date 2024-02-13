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
        private IReadOnlyList<DateTime> _data;

        public MarketData(IDataSource dataSource)
        {
            var companies = dataSource.GetCompanies();

            Companies = companies.Select(c => c.Name).ToList();

            _data = companies.
                SelectMany(c => c.Data).
                Select(d => d.Date).
                Order().
                ToList();
        }

        public DateTime FirstEntryDate => _data.First();

        public DateTime LastEntryDate => _data.Last();

        public IEnumerable<CompanyName> Companies { get; private init; }
    }

    internal record CompanyName(string Name);

    internal record PriceAtTime(DateTime Date, double Price);

    internal record CompanyData(CompanyName Name, IEnumerable<PriceAtTime> Data);
}
