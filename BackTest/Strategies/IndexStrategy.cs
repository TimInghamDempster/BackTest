using BackTest.Data;
using BackTest.Framework;
using BackTest.Trading;

namespace BackTest.Strategies
{
    internal class IndexStrategy : IStrategy
    {
        private readonly int _companyCount;

        public IndexStrategy(int companyCount)
        {
            _companyCount = companyCount;
        }

        public Order GenerateInitialOrder(IMarketAtTime market, Portfolio portfolio)
        {
            var date = market.FirstEntryDate;
            IEnumerable<CompanyName> companies = GetTopCompanies(market, date);

            var moneyPerCompany = portfolio.Cash.Amount / companies.Count();

            return new(companies.Select(c => new Trade.Buy(
                    c,
                    (int)(moneyPerCompany / market.GetPriceAtTime(c, date).Price))));
        }

        private static IEnumerable<CompanyName> GetTopCompanies(IMarketAtTime market, DateTime date)=>
            market.Companies
                .Where(c => market.GetPriceAtTime(c, date).Price > 0)
                .OrderBy(c => market.GetPriceAtTime(c, date).Price)
                .Take(100);

        public Order GenerateOrder(IMarketAtTime market, DateTime date, Portfolio portfolio)
        {
            // Would loose a lot of money to thrashing so debounce
            var daysBack = 5;
            if (date - market.FirstEntryDate < TimeSpan.FromDays(daysBack))
            {
                return new Order(new List<Trade>());
            }

            var currentTopCompanies = GetTopCompanies(market, date);
            for(int i = 0; i < daysBack - 1; i++)
            {
                var previousTopCompanies = GetTopCompanies(market, date - TimeSpan.FromDays(i)).ToList();
                currentTopCompanies = currentTopCompanies.Concat(previousTopCompanies);
            }

            currentTopCompanies = currentTopCompanies.Distinct();

            var toSell = portfolio.Stocks.Where(s => !currentTopCompanies.Any(c => c == s.Name))
                .Select(s => new Trade.Sell(s.Name, s.Amount));

            var companiesToBuy = currentTopCompanies.Where(c => !portfolio.Stocks.Any(s => s.Name == c));

            var proceeds = toSell.Select(s => s.Amount * market.GetPriceAtTime(s.Name, date).Price).Sum();

            var moneyPerCompany = (portfolio.Cash.Amount + proceeds) / _companyCount;

            var toBuy = companiesToBuy.Select(c => new Trade.Buy(
                c,
                (int)(moneyPerCompany / market.GetPriceAtTime(c, date).Price)))
                .Where(t => t.Amount > 0)
                .Select(t => t as Trade);

            return new Order(toBuy.Concat(toSell));
        }
    }
}
