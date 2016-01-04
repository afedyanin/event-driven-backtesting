namespace BackTesting.Model.DataHandlers
{
    using System;
    using System.Collections.Generic;
    using Deedle;

    public interface IDataHandler
    {
        ICollection<string> Symbols { get; }
        bool ContinueBacktest { get; }
        DateTime? CurrentTime { get; }
        ObjectSeries<string> GetLast(string symbol);
        decimal? GetLastClosePrice(string symbol);
        void Update();
    }
}
