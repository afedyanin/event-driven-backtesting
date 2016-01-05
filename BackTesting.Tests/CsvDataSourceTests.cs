namespace BackTesting.Tests
{
    using System.Linq;
    using BackTesting.Model.MarketData;
    using BackTesting.Model.Utils;
    using NUnit.Framework;

    [TestFixture]
    public class CsvDataSourceTests
    {
        [Test]
        public void DataSourceLoadedFromStringHasValidFrames()
        {
            var dataSource = CsvDataSource.CreateFormStrings(Mother.UnsortedRowsCsvData);
            Assert.IsNotNull(dataSource.Bars);
            Assert.AreEqual(2, dataSource.Bars.Keys.Count);
            Assert.Contains(Symbols.Sber, dataSource.Bars.Keys.ToList());
            Assert.Contains(Symbols.Vtbr, dataSource.Bars.Keys.ToList());
        }

        [Test]
        public void DataSourceLoadedFromDailyScvIsValid()
        {
            var dataSource = CsvDataSource.CreateFormStrings(Mother.DailyCsvData);
            Assert.IsNotNull(dataSource.Bars);
            Assert.AreEqual(1, dataSource.Bars.Keys.Count);
            Assert.Contains(Symbols.Sber, dataSource.Bars.Keys.ToList());

            var sberBar = dataSource.Bars[Symbols.Sber];
            Assert.IsTrue(sberBar.Keys.Any());
        }

        [Test]
        public void String2FrameTransformIsValid()
        {
            var csvString = Mother.DailyCsvData.First().Value;
            var bars = Csv2Frame.LoadBarsFromString(csvString);
            Assert.IsTrue(bars.Count > 0);
        }
    }
}
