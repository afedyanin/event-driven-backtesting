namespace BackTesting.Tests
{
    using System.Linq;
    using BackTesting.Model.MarketData;
    using BackTesting.Model.Utils;
    using Deedle;
    using NUnit.Framework;

    [TestFixture]
    public class CsvDataSourceTests
    {

        [Test]
        public void DataSourceLoadedFromStringHasValidFrames()
        {
            var dataSource = CsvDataSource.CreateFormStrings(Mother.UnsortedRowsCsvData);
            Assert.IsNotNull(dataSource.Frames);
            Assert.AreEqual(2, dataSource.Frames.Keys.Count);
            Assert.Contains(Symbols.Sber, dataSource.Frames.Keys.ToList());
            Assert.Contains(Symbols.Vtbr, dataSource.Frames.Keys.ToList());
        }

        [Test]
        public void DataSourceLoadedFromDailyScvIsValid()
        {
            var dataSource = CsvDataSource.CreateFormStrings(Mother.DailyCsvData);
            Assert.IsNotNull(dataSource.Frames);
            Assert.AreEqual(1, dataSource.Frames.Keys.Count);
            Assert.Contains(Symbols.Sber, dataSource.Frames.Keys.ToList());

            var sberFrame = dataSource.Frames[Symbols.Sber];
            Assert.IsTrue(sberFrame.RowKeys.Any());
        }

        [Test]
        public void String2FrameTransformIsValid()
        {
            var csvString = Mother.DailyCsvData.First().Value;
            var frame = Csv2Frame.LoadFromString(csvString);
            Assert.IsTrue(frame.RowCount > 0);
            frame.Print();
        }
    }
}
