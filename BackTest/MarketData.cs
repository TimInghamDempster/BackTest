namespace BackTest
{
    internal interface IMarketData
    {
        public DateTime FirstEntryDate { get; }
        public DateTime LastEntryDate { get; }
    }

    internal class MarketData : IMarketData
    {
        public DateTime FirstEntryDate => throw new NotImplementedException();

        public DateTime LastEntryDate => throw new NotImplementedException();
    }
}
