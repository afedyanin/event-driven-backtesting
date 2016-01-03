namespace BackTesting.Model.MarketData
{
    using System;
    using System.Collections.Generic;
    using Deedle;

    public interface IMarketData
    {
        IDictionary<string, Frame<DateTime, string>> Bars { get; }
        IList<DateTime> RowKeys { get; }
        ICollection<string> Symbols { get; }
    }
}
