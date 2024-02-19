using BackTest.Data;
using BackTest.Framework;

namespace BackTest.Trading
{
    internal record struct TraderName(string Value);
    internal class Trader : IPriceSeries
    {
        private readonly TraderName _name;
        private Portfolio _portfolio;
        private readonly IMarketAtTime _market;

        public Trader(Portfolio portfolio, TraderName Name, IMarketAtTime market)
        {
            _portfolio = portfolio;
            _name = Name;
            _market = market;
        }

        public string Name => _name.Value;

        public PriceAtTime Price(DateTime date) =>
            _portfolio.Evaluate(_market, date);
    }
}
