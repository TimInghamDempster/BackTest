using BackTest.Data;

namespace BackTest.Framework
{
    internal interface IPriceSeries
    {
        string Name { get; }
        PriceAtTime Price(DateTime date);
    }
}
