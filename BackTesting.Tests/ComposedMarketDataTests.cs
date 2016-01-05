namespace BackTesting.Tests
{
    using System;
    using System.Linq;
    using BackTesting.Model.MarketData;
    using NUnit.Framework;

    [TestFixture]
    public class ComposedMarketDataTests
    {
        [Test]
        public void ComposedMarketDataLoadedFromStringsHasValidRowKeys()
        {
            var dataSource = CsvDataSource.CreateFormStrings(Mother.UnsortedRowsCsvData);
            var mdata = new ComposedMarketData(dataSource.Bars);
            var rowKeys = mdata.RowKeys;

            foreach (var dateTime in rowKeys)
            {
                Console.WriteLine("key={0}", dateTime);
            }

            DateTime? prev = null;
            foreach (var dateTime in rowKeys)
            {
                if (prev == null)
                {
                    prev = dateTime;
                    continue;
                }

                Assert.IsTrue(prev <= dateTime);
            }
        }

        [Test]
        public void ComposedMarketDataLoadedFromStringsIsValid()
        {
            var dataSource = CsvDataSource.CreateFormStrings(Mother.UnsortedRowsCsvData);
            var mdata = new ComposedMarketData(dataSource.Bars);

            Assert.IsNotNull(mdata.Bars);
            Assert.IsTrue(mdata.RowKeys.Count > 0);
            Assert.IsTrue(mdata.Symbols.Count == 2);
            Assert.Contains(Symbols.Sber, mdata.Symbols.ToList());
            Assert.Contains(Symbols.Vtbr, mdata.Symbols.ToList());

            var sberBars = mdata.Bars[Symbols.Sber];

            foreach (var bar in sberBars.Values)
            {
                Console.WriteLine(bar);
            }
        }
    }
}
