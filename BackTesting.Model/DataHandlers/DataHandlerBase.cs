namespace BackTesting.Model.DataHandlers
{
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
    public abstract class DataHandlerBase
    {
        public abstract ICollection<string> Symbols { get; }

        public abstract bool ContinueBacktest { get; }

        /// <summary>
        /// Returns the last N bars from the latest_symbol list, or fewer if less bars are available.
        /// </summary>
        public abstract IEnumerable<ObjectSeries<string>> GetLatestBars(string symbol, int n = 1);

        public abstract ObjectSeries<string> GetLast(string symbol);

        /// <summary>
        /// Pushes the latest bar to the latest symbol structure for all symbols in the symbol list.
        /// </summary>
        public abstract void UpdateBars();
    }
}
