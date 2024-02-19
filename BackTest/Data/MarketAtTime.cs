namespace BackTest.Data
{
    internal interface IMarketAtTime : IMarketData;

    internal class MarketAtTime : IMarketAtTime
    {
        private readonly IMarketData _marketData;
        private DateTime _date;

        internal MarketAtTime(IMarketData marketData)
        {
            _marketData = marketData;
            _date = _marketData.FirstEntryDate;
        }

        public void SetDate(DateTime date)
        {
            _date = date;
        }

        public DateTime FirstEntryDate => _marketData.FirstEntryDate;

        public DateTime LastEntryDate => _date;

        public IEnumerable<CompanyName> Companies => _marketData.Companies;

        public PriceAtTime GetPriceAtTime(CompanyName name, DateTime date) =>
            date <= _date ?
            _marketData.GetPriceAtTime(name, date) :
            throw new ArgumentOutOfRangeException(nameof(date), "Date is after the current date");
    }
}
