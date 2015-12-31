namespace BackTesting.Model.Entities
{
    using System;
    using Deedle;

    public interface IMarketData
    {
        Frame<DateTime, string> GetBars(string symbol);
    }
}
