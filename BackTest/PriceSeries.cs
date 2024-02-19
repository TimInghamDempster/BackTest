namespace BackTest
{
    internal interface IPriceSeries
    {
        string Name { get; }
        PriceAtTime Price(DateTime date);
    }
}
