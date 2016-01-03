namespace BackTesting.Model.DataHandlers
{
    using System;
    using System.Collections.Generic;
    using BackTesting.Model.MarketData;
    using Deedle;
    using Events;

    /// <summary>
    /// HistoricCSVDataHandler is designed to read CSV files for
    /// each requested symbol from disk and provide an interface
    /// to obtain the "latest" bar in a manner identical to a live
    /// trading interface. 
    /// </summary>
    public class HistoricDataHandler : IDataHandler
    {
        private readonly IEventBus eventBus;
        private readonly IMarketData marketData;
        private readonly IEnumerator<DateTime> timeEnumerator;

        private DateTime? currentTime;

        public bool ContinueBacktest { get; private set; }

        public ICollection<string> Symbols => this.marketData.Symbols;

        public HistoricDataHandler(IEventBus eventBus, IMarketData marketData)
        {
            this.eventBus = eventBus;
            this.marketData = marketData;
            this.ContinueBacktest = true;
            this.timeEnumerator = this.marketData.RowKeys.GetEnumerator();
            this.currentTime = null;
        }

        public ObjectSeries<string> GetLast(string symbol)
        {
            var bars = this.marketData.Bars[symbol];

            if (bars == null)
            {
                return null;
            }

            if (!this.currentTime.HasValue)
            {
                return null;
            }

            return bars.Rows.Get(this.currentTime.Value, Lookup.ExactOrSmaller);
        }

        public void Update()
        {
            if (!this.ContinueBacktest)
            {
                return;
            }

            this.currentTime = this.GetNextTime();

            if (this.currentTime.HasValue)
            {
                this.eventBus.Put(new MarketEvent(this.currentTime.Value));
            }
            else
            {
                this.ContinueBacktest = false;
            }
        }

        private DateTime? GetNextTime()
        {
            var moved = this.timeEnumerator.MoveNext();
            return moved ? this.timeEnumerator.Current : (DateTime?)null;
        }
    }
}
