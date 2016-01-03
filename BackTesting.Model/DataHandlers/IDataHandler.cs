namespace BackTesting.Model.DataHandlers
{
    using System;
    using System.Collections.Generic;
    using Deedle;

    /// <summary>
    /// DataHandler is an abstract base class providing an interface for
    /// all subsequent(inherited) data handlers(both live and historic).
    /// The goal of a(derived) DataHandler object is to output a generated
    /// set of bars(OLHCVI) for each symbol requested.
    /// This will replicate how a live strategy would function as current
    /// market data would be sent "down the pipe". Thus a historic and live
    /// system will be treated identically by the rest of the backtesting suite.
    /// </summary>
    public interface IDataHandler
    {
        ICollection<string> Symbols { get; }
        bool ContinueBacktest { get; }
        DateTime? CurrentTime { get; }
        ObjectSeries<string> GetLast(string symbol);
        void Update();
    }
}
