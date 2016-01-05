namespace BackTesting.Model.MarketData
{
    using System;
    using System.Collections.Generic;

    public interface IMarketData
    {
        IDictionary<string, IDictionary<DateTime, Bar>> Bars { get; }
        IList<DateTime> RowKeys { get; }
        ICollection<string> Symbols { get; }
    }
}
