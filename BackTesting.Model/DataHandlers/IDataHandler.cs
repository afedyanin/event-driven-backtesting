namespace BackTesting.Model.DataHandlers
{
    using System;
    using System.Collections.Generic;
    using BackTesting.Model.MarketData;

    public interface IDataHandler
    {
        ICollection<string> Symbols { get; }
        bool ContinueBacktest { get; }
        DateTime? CurrentTime { get; }
        Bar GetLast(string symbol);
        decimal? GetLastClosePrice(string symbol);
        void Update();
    }
}
