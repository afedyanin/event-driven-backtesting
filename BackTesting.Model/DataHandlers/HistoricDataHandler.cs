namespace BackTesting.Model.DataHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BackTesting.Model.Entities;
    using Deedle;
    using Events;

    /// <summary>
    /// HistoricCSVDataHandler is designed to read CSV files for
    /// each requested symbol from disk and provide an interface
    /// to obtain the "latest" bar in a manner identical to a live
    /// trading interface. 
    /// </summary>
    public class HistoricDataHandler : DataHandlerBase
    {
        private readonly IEventBus eventBus;
        private readonly IMarketData marketData;
        private readonly IDictionary<string, Frame<DateTime, string>> latestBars;
        private readonly IEnumerator<DateTime> timeEnumerator;

        private bool continueBacktest;

        public override ICollection<string> Symbols => this.marketData.Symbols;

        public override bool ContinueBacktest => this.continueBacktest;

        public HistoricDataHandler(IEventBus eventBus, IMarketData marketData)
        {
            this.eventBus = eventBus;
            this.marketData = marketData;
            this.continueBacktest = true;
            this.timeEnumerator = this.marketData.RowKeys.GetEnumerator();
            this.latestBars = new Dictionary<string, Frame<DateTime, string>>();
        }

        public override IEnumerable<ObjectSeries<string>> GetLatestBars(string symbol, int n = 1)
        {
            var bars = this.latestBars[symbol];
            return bars.Rows.Values.Reverse().Take(n);
        }

        public override ObjectSeries<string> GetLast(string symbol)
        {
            var bars = this.latestBars[symbol];
            return bars.Rows.Values.Reverse().First();
        }

        // Pushes the latest bar to the latestBars structure
        // for all symbols in the symbol list.
        public override void UpdateBars()
        {
            if (!this.ContinueBacktest)
            {
                return;
            }

            var nextTime = this.GetNextTime();

            if (nextTime == null)
            {
                this.continueBacktest = false;
                return;
            }

            this.AppendLatestBars(nextTime.Value);
            this.eventBus?.Put(new MarketEvent(nextTime.Value));
        }

        private DateTime? GetNextTime()
        {
            var moved = this.timeEnumerator.MoveNext();
            return moved ? this.timeEnumerator.Current : (DateTime?)null;
        }

        private void AppendLatestBars(DateTime nextTime)
        {
            foreach (var symbol in this.Symbols)
            {
                var bar = this.marketData.GetBars(symbol).Rows[nextTime];

                if (!this.latestBars.ContainsKey(symbol))
                {
                    this.latestBars.Add(symbol, Frame.CreateEmpty<DateTime, string>());
                }

                this.latestBars[symbol] = this.AppendBar(this.latestBars[symbol], nextTime, bar);
            }
        }

        private Frame<DateTime, string> AppendBar(Frame<DateTime, string> targetFrame, DateTime key, ObjectSeries<string> bar)
        {
            var newData = new List<KeyValuePair<DateTime, ObjectSeries<string>>>()
            {
                new KeyValuePair<DateTime, ObjectSeries<string>>(key, bar)
            };

            var series = new Series<DateTime, ObjectSeries<string>>(newData);
            var merged = targetFrame.Rows.Merge(series, UnionBehavior.PreferRight);
            return Frame.FromRows(merged).SortRowsByKey();
        }
    }
}
