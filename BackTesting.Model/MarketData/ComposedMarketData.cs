namespace BackTesting.Model.MarketData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Deedle;

    public class ComposedMarketData : IMarketData
    {
        public IList<DateTime> RowKeys { get; private set; }
        public ICollection<string> Symbols => this.Bars.Keys;
        public IDictionary<string, Frame<DateTime, string>> Bars { get; }

        public ComposedMarketData(IDictionary<string, Frame<DateTime, string>> frames)
        {
            this.Bars = new Dictionary<string, Frame<DateTime, string>>();
            this.RowKeys = this.ComposeRowKeys();
        }

        private IList<DateTime> ComposeRowKeys()
        {
            var res = new List<DateTime>();
            return this.Bars.Keys.Aggregate(res, (current, symbol) => UnionRowKeys(current, this.Bars[symbol].RowKeys).OrderBy(k => k).ToList());
        } 

        private static IEnumerable<DateTime> UnionRowKeys(IEnumerable<DateTime> source1, IEnumerable<DateTime> source2)
        {
            return source1?.Union(source2) ?? source2;
        }
    }
}
