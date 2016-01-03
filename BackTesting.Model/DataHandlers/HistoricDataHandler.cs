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

        public DateTime? CurrentTime { get; private set; }
        public bool ContinueBacktest { get; private set; }

        public ICollection<string> Symbols => this.marketData.Symbols;

        public HistoricDataHandler(IEventBus eventBus, IMarketData marketData)
        {
            this.eventBus = eventBus;
            this.marketData = marketData;
            this.ContinueBacktest = true;
            this.timeEnumerator = this.marketData.RowKeys.GetEnumerator();
            this.CurrentTime = null;
        }

        public ObjectSeries<string> GetLast(string symbol)
        {
            var bars = this.marketData.Bars[symbol];

            if (bars == null)
            {
                return null;
            }

            if (!this.CurrentTime.HasValue)
            {
                return null;
            }

            var res = bars.Rows.TryGet(this.CurrentTime.Value, Lookup.ExactOrSmaller);

            return res.HasValue ? res.Value : null;
        }

        public void Update()
        {
            if (!this.ContinueBacktest)
            {
                return;
            }

            this.CurrentTime = this.GetNextTime();

            if (this.CurrentTime.HasValue)
            {
                this.eventBus.Put(new MarketEvent(this.CurrentTime.Value));
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
