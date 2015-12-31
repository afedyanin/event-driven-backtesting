namespace BackTesting.Model.Entities
{
    using System;
    using System.Collections.Generic;
    using Deedle;

    public interface IMarketData
    {
        IList<DateTime> RowKeys { get; }
        ICollection<string> Symbols { get; }
        Frame<DateTime, string> GetBars(string symbol);
    }
}
