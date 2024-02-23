using BackTest.Data;
using BackTest.Framework;
using BackTest.Strategies;

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
            var initialOrder = strategy.GenerateInitialOrder(market, portfolio);
            _portfolio = ExecuteOrder(initialOrder, portfolio, market);
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
            _portfolio = ExecuteOrder(order, _portfolio, _market);
        }

        private static Portfolio ExecuteOrder(Order order, Portfolio portfolio, IMarketAtTime market)
        {
            foreach (var trade in order.Trades)
            {
                portfolio =
                    portfolio.Execute(trade, market).Match(
                        s => s,
                        f => portfolio);
            }
            return portfolio;
        }

        internal static IPriceSeries IndexTrader(IMarketAtTime marketAtTime, int startingCapital, int companyCount)
        {
            var portfolio = new Portfolio(new(startingCapital), new List<Stock>());

            return new Trader(portfolio, new("Index"), marketAtTime, new IndexStrategy(companyCount));
        }
    }
}
