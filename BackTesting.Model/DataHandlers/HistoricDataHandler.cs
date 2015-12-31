namespace BackTesting.Model.DataHandlers
{
    using System;
    using System.Collections;
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

        private IDictionary<string, Frame<DateTime, string>> latestBars;
        private readonly IEnumerator<DateTime> timeEnumerator; 

        public bool ContinueBacktest { get; set; }

        public HistoricDataHandler(IEventBus eventBus, IMarketData marketData)
        {
            this.eventBus = eventBus;
            this.marketData = marketData;
            this.ContinueBacktest = true;
            this.timeEnumerator = this.marketData.RowKeys.GetEnumerator();
            this.latestBars = new Dictionary<string, Frame<DateTime, string>>();
        }

        public override void GetLatestBars(string symbol, int n = 1)
        {
        }

        // Pushes the latest bar to the latestBars structure
        // for all symbols in the symbol list.
        public override void UpdateBars()
        {
            var nextTime = this.GetNextTime();

            if (nextTime == null)
            {
                this.ContinueBacktest = false;
                return;
            }

            foreach (var symbol in this.marketData.Symbols)
            {
                var bar = this.marketData.GetBars(symbol).Rows[nextTime.Value];

                if (!this.latestBars.ContainsKey(symbol))
                {
                    this.latestBars.Add(symbol, Frame.CreateEmpty<DateTime, string>());
                }

                this.latestBars[symbol] = this.Append(this.latestBars[symbol], nextTime.Value, bar);
            }
        }

        private DateTime? GetNextTime()
        {
            var moved = this.timeEnumerator.MoveNext();
            return moved ? this.timeEnumerator.Current : (DateTime?)null;
        }

        private Frame<DateTime, string> Append(Frame<DateTime, string> targetFrame, DateTime key, ObjectSeries<string> bar)
        {
            var newData = new List<KeyValuePair<DateTime, ObjectSeries<string>>>()
            {
                new KeyValuePair<DateTime, ObjectSeries<string>>(key, bar)
            };

            var series = new Series<DateTime, ObjectSeries<string>>(newData);
            var merged = targetFrame.Rows.Merge(series, UnionBehavior.PreferRight);
            return Frame.FromRows(merged);
        }

        // TODO: remove it
        public Frame<DateTime, string> GetAllBars(string symbol)
        {
            return this.marketData.GetBars(symbol);
        }
    }
}
