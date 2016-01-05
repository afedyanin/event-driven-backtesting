namespace BackTesting.Tests
{
    using System;
    using BackTesting.Model.MarketData;
    using BackTesting.Model.Utils;
    using NUnit.Framework;

    [TestFixture]
    public class Csv2FrameTests
    {
        [Test]
        public void CanLoadBarsFromCsvString()
        {
            var bars = Csv2Frame.LoadBarsFromString(Mother.GenericCsvData[Symbols.Sber]);

            Assert.IsTrue(bars.Count > 0);

            foreach (var kvp in bars)
            {
                Console.WriteLine($"{kvp.Key} => {kvp.Value}");
            }
        }
    }
}
