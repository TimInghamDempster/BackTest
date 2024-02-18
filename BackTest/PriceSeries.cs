namespace BackTest
{
    internal interface IPriceSeries
    {
        string Name { get; }
        PriceAtTime Price(DateTime date);
    }

    internal class Index : IPriceSeries
    {
        private readonly IMarketAtTime _market;
        private readonly Func<IMarketAtTime, DateTime, PriceAtTime> _calculate;

        public Index(string name, IMarketAtTime market, Func<IMarketAtTime, DateTime, PriceAtTime> calculate)
        {
            Name = name;
            _market = market;
            _calculate = calculate;
        }
        public string Name { get; init; }

        public PriceAtTime Price(DateTime date)
        {
            var res = _calculate(_market, date);
            return res;
        }

        public static Index WholeMarket(IMarketAtTime market) =>
            new("Whole Market", market, (m, d) =>
            new(m.Companies.Select(c => m.GetPriceAtTime(c, d).Price).Sum()));
        public static Index Take(IMarketAtTime market, int count) =>
           new($"Take {count}", market, (m, d) =>
           new(m.Companies.Take(count).Select(c => m.GetPriceAtTime(c, d).Price).Sum()));
    }
}
