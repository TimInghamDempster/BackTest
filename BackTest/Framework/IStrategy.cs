using BackTest.Data;
using BackTest.Trading;

namespace BackTest.Framework
{
    internal record struct Order(IEnumerable<Trade> Trades);

    internal interface IStrategy
    {
        Order GenerateOrder(IMarketAtTime market, DateTime date, Portfolio portfolio);
    }
}
