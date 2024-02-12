namespace BackTest
{
    internal interface IMarketData
    {
        public DateTime FirstEntryDate { get; }
        public DateTime LastEntryDate { get; }
    }

    internal class MarketData : IMarketData
    {
        private IReadOnlyList<DateTime> _data;

        public MarketData(IDataSource dataSource)
        {
            _data = dataSource.GetCompanies().SelectMany(c => c.Dates).Order().ToList();
        }

        public DateTime FirstEntryDate => _data.First();

        public DateTime LastEntryDate => _data.Last();
    }

    internal record CompanyData(IEnumerable<DateTime> Dates);
}
