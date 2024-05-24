using BackTest.Data;
using BackTest.Framework;
using BackTest.Trading;

namespace BackTest.Strategies
{
    internal class IndexStrategyWithBuffer : IStrategy
    {
        private readonly int _companyCount;
        private readonly int _rebalanceDays;
        private DateTime _dateOfLastRebalance;

        public IndexStrategyWithBuffer(int companyCount, int rebalanceDays)
        {
            _companyCount = companyCount;
            _rebalanceDays = rebalanceDays;
        }

        public Order GenerateOrder(IMarketAtTime market, DateTime date, Portfolio portfolio)
        {
            if ((date - _dateOfLastRebalance).Days > _rebalanceDays)
            {
                _dateOfLastRebalance = date;
                return RebalancePortfolio(market, portfolio, date);
            }
            return new Order(new List<Trade>());
        }

        private record struct PriceData(CompanyName Name, double Price);

        private Order RebalancePortfolio(IMarketAtTime market, Portfolio portfolio, DateTime date)
        {
            var topCompanies = market.Companies
                .Select(c => new PriceData(Name: c, Price: market.GetPriceAtTime(c, date).Price))
                .OrderByDescending(p =>p.Price)
                .Where(p => p.Price > 0)
                .Take((int)(_companyCount * 2.0))
                .ToDictionary(pd => pd.Name, pd => pd.Price);

            var toBuyFrom = topCompanies.Take(_companyCount).ToDictionary();

            var toSell = portfolio.Stocks
                .Where(s => !topCompanies.ContainsKey(s.Name));

            var sellOrders = toSell
                .Select(s => new Trade.Sell(s.Name, portfolio.Stocks.First(ps => ps.Name == s.Name).Amount) as Trade);

            var saleValue = toSell
                .Select(s => market.GetPriceAtTime(s.Name, date).Price * portfolio.Stocks.First(ps => ps.Name == s.Name).Amount)
                .Sum();

            var toBuy = toBuyFrom
                .Where(t => !portfolio.Stocks.Any(s => s.Name == t.Key));

            var costForOneOfEach = toBuy
                .Select(c => c.Value)
                .Sum();

            var freeCash = portfolio.Cash.Amount + saleValue;
            var amount = (int)(freeCash / costForOneOfEach);

            var buyOrders = toBuy
                .Select(t => new Trade.Buy(t.Key, amount));

            var saleCount = sellOrders.Count();

            return new Order(sellOrders.Concat(buyOrders));
        }
    }

    internal class IndexStrategyPartialRebalance : IStrategy
    {
        private readonly int _companyCount;
        private readonly int _rebalanceDays;
        private DateTime _dateOfLastRebalance;

        public IndexStrategyPartialRebalance(int companyCount, int rebalanceDays)
        {
            _companyCount = companyCount;
            _rebalanceDays = rebalanceDays;
        }

        public Order GenerateOrder(IMarketAtTime market, DateTime date, Portfolio portfolio)
        {
            if ((date - _dateOfLastRebalance).Days > _rebalanceDays)
            {
                _dateOfLastRebalance = date;
                return RebalancePortfolio(market, portfolio, date);
            }
            return new Order(new List<Trade>());
        }

        private record struct PriceData(CompanyName Name, double Price);

        private Order RebalancePortfolio(IMarketAtTime market, Portfolio portfolio, DateTime date)
        {
            var topCompanies = market.Companies
                .Select(c => new PriceData(Name: c, Price: market.GetPriceAtTime(c, date).Price))
                .OrderByDescending(p => p.Price)
                .Where(p => p.Price > 0)
                .Take(_companyCount)
                .ToDictionary(pd => pd.Name, pd => pd.Price);

            var toBuyFrom = topCompanies.Take(_companyCount).ToDictionary();

            var toSell = portfolio.Stocks
                .Where(s => !topCompanies.ContainsKey(s.Name));

            var sellOrders = toSell
                .Select(s => new Trade.Sell(s.Name, portfolio.Stocks.First(ps => ps.Name == s.Name).Amount) as Trade);

            var saleValue = toSell
                .Select(s => market.GetPriceAtTime(s.Name, date).Price * portfolio.Stocks.First(ps => ps.Name == s.Name).Amount)
                .Sum();

            var toBuy = toBuyFrom
                .Where(t => !portfolio.Stocks.Any(s => s.Name == t.Key));

            var costForOneOfEach = toBuy
                .Select(c => c.Value)
                .Sum();

            var freeCash = portfolio.Cash.Amount + saleValue;
            var amount = (int)(freeCash / costForOneOfEach);

            var buyOrders = toBuy
                .Select(t => new Trade.Buy(t.Key, amount));

            var saleCount = sellOrders.Count();

            return new Order(sellOrders.Concat(buyOrders));
        }
    }

    internal class IndexStrategyNaive : IStrategy
    {
        private readonly int _companyCount;
        private readonly int _rebalanceDays;
        private DateTime _dateOfLastRebalance;

        public IndexStrategyNaive(int companyCount, int rebalanceDays)
        {
            _companyCount = companyCount;
            _rebalanceDays = rebalanceDays;
        }

        public Order GenerateOrder(IMarketAtTime market, DateTime date, Portfolio portfolio)
        {
            if ((date - _dateOfLastRebalance).Days > _rebalanceDays)
            {
                _dateOfLastRebalance = date;
                return RebalancePortfolio(market, portfolio, date);
            }
            return new Order(new List<Trade>());
        }

        private Order RebalancePortfolio(IMarketAtTime market, Portfolio portfolio, DateTime date)
        {
            var topCompanies = market.Companies
                .OrderByDescending(c => market.GetPriceAtTime(c, market.LastEntryDate).Price)
                .Where(c => market.GetPriceAtTime(c, market.LastEntryDate).Price > 0)
                .Take(_companyCount);

            var portfolioValue = portfolio.Evaluate(market, date);

            var moneyPerCompany = portfolioValue.Price / topCompanies.Count();

            var targetVolumes = topCompanies.Select(c => new
            {
                Company = c,
                TargetVolume = (int)(moneyPerCompany / market.GetPriceAtTime(c, date).Price)
            });

            var sellOrders = portfolio.Stocks
                .Select(s =>
                {
                    var amount = Math.Max(s.Amount - (targetVolumes.FirstOrDefault(
                        t => t.Company == s.Name)?.TargetVolume ?? 0), 0);

                    return new Trade.Sell(s.Name, amount);
                })
                .Where(o => o.Amount > 0)
                .Select(o => o as Trade);

            var buyOrders = targetVolumes
                .Select(t =>
                {
                    var amount = Math.Max(t.TargetVolume - portfolio.Stocks.FirstOrDefault(
                        s => s.Name == t.Company).Amount, 0);

                    return new Trade.Buy(t.Company, amount);
                })
                .Where(o => o.Amount > 0);

            return new Order(sellOrders.Concat(buyOrders));
        }
    }
}
