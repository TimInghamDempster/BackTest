using BackTest.Data;

namespace BackTest.Framework
{
    internal interface IPriceSeriesCollection
    {
        IEnumerable<IPriceSeries> Series { get; }

    }
    
    internal interface IPriceSeries
    {
        string Name { get; }
        PriceAtTime Price(DateTime date);
    }
}
