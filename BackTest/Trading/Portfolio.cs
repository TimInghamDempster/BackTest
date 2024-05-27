using BackTest.Data;
using LanguageExt.Common;

namespace BackTest.Trading
{
    internal record struct Cash(double Amount);
    internal record struct Stock(CompanyName Name, int Amount);

    internal record Portfolio(Cash Cash, IReadOnlyList<Stock> Stocks);

    internal abstract record Trade
    {
        internal record Buy(CompanyName Name, int Amount) : Trade;
        internal record Sell(CompanyName Name, int Amount) : Trade;
    }

    internal static class PortflioExtensions
    {
        internal static Result<Portfolio> Execute(
            this Portfolio portfolio, Trade trade, IMarketAtTime market) =>
            trade switch
            {
                Trade.Buy buy => portfolio.Buy(buy, market),
                Trade.Sell sell => portfolio.Sell(sell, market),
                _ => throw new ArgumentOutOfRangeException(nameof(trade))
            };

        private static Result<Portfolio> Sell(this Portfolio portfolio, Trade.Sell sell, IMarketAtTime market)
        {
            var price = market.GetPriceAtTime(sell.Name, market.LastEntryDate).Price;
            if (!portfolio.Stocks.Any(s => s.Name == sell.Name))
            {
                return new(new ArgumentOutOfRangeException(nameof(sell), "Stock Not in Portfolio"));
            }
            var newStocks = portfolio.Stocks.ToList();
            var stock = newStocks.FirstOrDefault(s => s.Name == sell.Name);
            if (stock.Amount < sell.Amount)
            {
                return new(new ArgumentOutOfRangeException(nameof(sell), "Not enough stocks"));
            }
            if(sell.Amount < 0)
            {
                return new(new ArgumentOutOfRangeException(nameof(sell), "Amount must be positive"));
            }
            newStocks.Remove(stock);

            stock = stock with { Amount = stock.Amount - sell.Amount };

            if (stock.Amount != 0)
            {
                newStocks.Add(stock);
            }
            return portfolio with { Cash = new(portfolio.Cash.Amount + price * sell.Amount), Stocks = newStocks };
        }

        private static Result<Portfolio> Buy(this Portfolio portfolio, Trade.Buy buy, IMarketAtTime market)
        {
            var price = market.GetPriceAtTime(buy.Name, market.LastEntryDate).Price;

            if (price == 0)
            {
                return new(new ArgumentOutOfRangeException(nameof(buy), "Stock Not in Portfolio"));
            }

            var cost = price * buy.Amount;
            if (cost > portfolio.Cash.Amount)
            {
                return new(new ArgumentOutOfRangeException(nameof(buy), "Not enough cash"));
            }
            if (!market.Companies.Any(s => s == buy.Name))
            {
                return new(new ArgumentOutOfRangeException(nameof(buy), "Stock Not in Portfolio"));
            }
            if(buy.Amount < 0)
            {
                return new(new ArgumentOutOfRangeException(nameof(buy), "Amount must be positive"));
            }
            var newStocks = portfolio.Stocks.ToList();
            var stock = newStocks.FirstOrDefault(s => s.Name == buy.Name);

            // If we don't already own any this will be the default
            // stock and not have a name set
            stock = stock with { Name = buy.Name };
           
            newStocks.Remove(stock);
            newStocks.Add(stock with { Amount = stock.Amount + buy.Amount });
            
            return portfolio with { Cash = new(portfolio.Cash.Amount - cost), Stocks = newStocks };
        }

        internal static PriceAtTime Evaluate(
            this Portfolio portfolio, IMarketAtTime market, DateTime date) =>
                new (portfolio.Cash.Amount +
                portfolio.Stocks.Sum(s => market.GetPriceAtTime(s.Name, date).Price * s.Amount));

    }
}
