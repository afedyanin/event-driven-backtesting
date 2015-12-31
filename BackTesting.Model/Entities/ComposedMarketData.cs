namespace BackTesting.Model.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BackTesting.Model.DataSource.Csv;
    using BackTesting.Model.Utils;
    using Deedle;

    public class ComposedMarketData : IMarketData
    {
        private IDictionary<string, Frame<DateTime, string>> bars;
        private IList<DateTime> rowKeys;

        protected ComposedMarketData()
        {
            this.bars = new Dictionary<string, Frame<DateTime, string>>();
            this.rowKeys = new List<DateTime>();
        }

        public static ComposedMarketData CreateFromCsv(CsvDataSource dataSource)
        {
            var mdata = new ComposedMarketData();
            foreach (var kvp in dataSource.Frames)
            {
                mdata.ComposeBars(kvp.Key, String2TimeSeries.Convert(kvp.Value));
            }

            return mdata;
        }

        public Frame<DateTime, string> GetBars(string symbol)
        {
            return !this.bars.ContainsKey(symbol) ? null : this.bars[symbol];
        }

        public void ComposeBars(string symbol, Frame<DateTime, string> frame)
        {
            if (this.bars.ContainsKey(symbol))
            {
                // todo: join with existing frame
                // todo: invalid operation exception
                //var existingFrame = this.bars[symbol];
                //var joined = existingFrame.Join(frame, JoinKind.Outer);
                //this.bars[symbol] = joined;

                this.bars[symbol] = frame;
            }
            else
            {
                this.bars.Add(symbol, frame);
            }

            this.rowKeys = UnionRowKeys(this.rowKeys, this.bars[symbol].RowKeys).ToList();
            this.bars = ReindexDataFrames(this.bars, this.rowKeys);
        }

        private static IEnumerable<DateTime> UnionRowKeys(IEnumerable<DateTime> source1, IEnumerable<DateTime> source2)
        {
            return source1?.Union(source2) ?? source2;
        }

        private static Dictionary<string, Frame<DateTime, string>> ReindexDataFrames(IDictionary<string, Frame<DateTime, string>> source, IList<DateTime> keys)
        {
            var res = new Dictionary<string, Frame<DateTime, string>>();

            foreach (var key in source.Keys)
            {
                var reindexedFrame = source[key].RealignRows(keys).SortRowsByKey();
                res.Add(key, reindexedFrame);
            }

            return res;
        }
    }
}
