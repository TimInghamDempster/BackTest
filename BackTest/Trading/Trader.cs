using BackTest.Data;
using BackTest.Framework;
using BackTest.Strategies;

namespace BackTest.Trading
{
    internal class PortfolioValue : IPriceSeries
    {
        private readonly Func<Portfolio> _portfolio;
        private readonly Func<IMarketAtTime> _market;
        private readonly TraderName _name;

        public PortfolioValue(Func<Portfolio> portfolio, Func<IMarketAtTime> market, TraderName name)
        {
            _portfolio = portfolio;
            _market = market;
            _name = name;
        }

        public string Name => $"{_name.Value} Portfolio value";

        public PriceAtTime Price(DateTime date) =>
            _portfolio().Evaluate(_market(), date);
    }

    internal class CashValue : IPriceSeries
    {
        private readonly Func<Portfolio> _portfolio;
        private readonly TraderName _name;

        public CashValue(Func<Portfolio> portfolio, TraderName name)
        {
            _portfolio = portfolio;
            _name = name;
        }

        public string Name => $"{_name.Value} Cash value";

        public PriceAtTime Price(DateTime date) =>
            new(_portfolio().Cash.Amount);
    }

    internal record struct TraderName(string Value);
    internal class Trader : IPriceSeriesCollection
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

            Series =
            [
                new PortfolioValue(() => _portfolio, () => _market, Name),
                //new CashValue(() => _portfolio, Name)
            ];
        }

        public string Name => _name.Value;

        public IEnumerable<IPriceSeries> Series { get; }

        public void Update(DateTime date)
        {
            var order = _strategy switch
            {
                IndexStrategyNaive index => index.GenerateOrder(_market, date, _portfolio),
                IndexStrategyPartialRebalance index => index.GenerateOrder(_market, date, _portfolio),
                IndexStrategyWithBuffer index => index.GenerateOrder(_market, date, _portfolio),
                IndexStrategyPriceWeighted index => index.GenerateOrder(_market, date, _portfolio),
                IndexStrategyNeutralWeigthRemainder index => index.GenerateOrder(_market, date, _portfolio),
                IndexStrategyWithBufferNeutral index => index.GenerateOrder(_market, date, _portfolio),
                IndexStrategyWithPrediction index => index.GenerateOrder(_market, date, _portfolio),
                IndexStrategyWithPredictionAndBuffer index => index.GenerateOrder(_market, date, _portfolio),
                _ => throw new NotImplementedException()
            };

            var value = _portfolio.Evaluate(_market, date);
            _portfolio = ExecuteOrder(order, _portfolio, _market);
            var valueAfter = _portfolio.Evaluate(_market, date);

            if (Math.Abs(value.Price - valueAfter.Price) > value.Price * 0.0001)
            {
                throw new Exception("Books don't balance");
            }

            if (date > new DateTime(2020, 1, 1))
            {
                var ordered = _portfolio
                    .Stocks
                    .OrderBy(s => _market.GetPriceAtTime(s.Name, date).Price)
                    .Select(s => (s.Name, s.Amount, _market.GetPriceAtTime(s.Name, date).Price))
                    .ToList();
                var total = ordered.Sum(s => s.Amount * s.Price);
            }

            _portfolio = Rationalise(_portfolio, date);
        }

        private Portfolio Rationalise(Portfolio portfolio, DateTime date)
        {
            return portfolio with
            {
                Stocks = portfolio.Stocks
                    .Where(s => _market.GetPriceAtTime(s.Name, date).Price > 0)
                    .ToList()
            };
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

        internal static Func<IStrategy, IPriceSeriesCollection> CreateTrader(IMarketAtTime marketAtTime, int startingCapital, int companyCount)
        {
            return (IStrategy strategy) =>
            {
                var portfolio = new Portfolio(new(startingCapital), new List<Stock>());

                return new Trader(portfolio, new(strategy.Name), marketAtTime, strategy);
            };
        }
 
        internal static IPriceSeriesCollection IndexTrader(IMarketAtTime marketAtTime, int startingCapital, int companyCount, int rebalancingPeriod)
        {
            var portfolio = new Portfolio(new(startingCapital), new List<Stock>());

            return new Trader(portfolio, new($"Index naive {rebalancingPeriod} days"), marketAtTime, new IndexStrategyNaive(companyCount, rebalancingPeriod));
        }

        internal static IPriceSeriesCollection IndexTrader2(IMarketAtTime marketAtTime, int startingCapital, int companyCount, int rebalancingPeriod)
        {
            var portfolio = new Portfolio(new(startingCapital), new List<Stock>());

            return new Trader(portfolio, new($"Index partial {rebalancingPeriod} days"), marketAtTime, new IndexStrategyPartialRebalance(companyCount, rebalancingPeriod));
        }


        internal static IPriceSeriesCollection IndexTrader3(IMarketAtTime marketAtTime, int startingCapital, int companyCount, int rebalancingPeriod)
        {
            var portfolio = new Portfolio(new(startingCapital), new List<Stock>());

            return new Trader(portfolio, new($"Index buffer {rebalancingPeriod} days"), marketAtTime, new IndexStrategyWithBuffer(companyCount, rebalancingPeriod));
        }
    }
}
