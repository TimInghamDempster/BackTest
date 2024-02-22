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
        private readonly IStrategy _strategy;

        public Trader(Portfolio portfolio, TraderName Name, IMarketAtTime market, IStrategy strategy)
        {
            _portfolio = portfolio;
            _name = Name;
            _market = market;
            _strategy = strategy;
        }

        public string Name => _name.Value;

        public PriceAtTime Price(DateTime date) =>
            _portfolio.Evaluate(_market, date);

        public void Update(DateTime date)
        {
            var order = _strategy.GenerateOrder(_market, date, _portfolio);

            foreach (var trade in order.Trades)
            {
                _portfolio =
                    _portfolio.Execute(trade, _market).Match(
                        s => s,
                        f => _portfolio);
            }
        }
    }
}
