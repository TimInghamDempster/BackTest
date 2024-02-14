namespace BackTest
{
    internal interface IPriceSeries
    {
        string Name { get; }
        PriceAtTime Price(DateTime date);
    }

    internal class DummyPriceSeries : IPriceSeries
    {
        public string Name => "Dummy";
        public PriceAtTime Price(DateTime date) =>
            new(10.0);
    }
}
