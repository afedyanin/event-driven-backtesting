namespace BackTesting.Model.DataHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BackTesting.Model.MarketData;
    using Events;

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

        public Bar GetLast(string symbol)
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

            var dateTime = this.CurrentTime.Value;

            if (bars.ContainsKey(dateTime))
            {
                return bars[dateTime];
            }

            var smallerDate = bars.Keys.OrderByDescending(k => k).FirstOrDefault(key => key <= dateTime);

            return bars.ContainsKey(smallerDate) ? bars[smallerDate] : null;
        }

        public decimal? GetLastClosePrice(string symbol)
        {
            return this.GetLast(symbol)?.Close;
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
