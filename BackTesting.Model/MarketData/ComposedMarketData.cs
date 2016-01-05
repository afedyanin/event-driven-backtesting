namespace BackTesting.Model.MarketData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ComposedMarketData : IMarketData
    {
        public IList<DateTime> RowKeys { get; private set; }
        public ICollection<string> Symbols => this.Bars.Keys;
        public IDictionary<string, IDictionary<DateTime, Bar>> Bars { get; }

        public ComposedMarketData(IDictionary<string, IDictionary<DateTime, Bar>> bars)
        {
            this.Bars = bars;
            this.RowKeys = this.ComposeRowKeys();
        }

        private IList<DateTime> ComposeRowKeys()
        {
            var res = new List<DateTime>();
            return this.Bars.Keys.Aggregate(res, (current, symbol) => UnionRowKeys(current, this.Bars[symbol].Keys).OrderBy(k => k).ToList());
        } 

        private static IEnumerable<DateTime> UnionRowKeys(IEnumerable<DateTime> source1, IEnumerable<DateTime> source2)
        {
            return source1?.Union(source2) ?? source2;
        }
    }
}
