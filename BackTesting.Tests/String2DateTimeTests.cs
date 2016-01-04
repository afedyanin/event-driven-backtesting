namespace BackTesting.Tests
{
    using System;
    using Model.Utils;
    using NUnit.Framework;

    [TestFixture]
    public class String2DateTimeTests
    {
        [Test]
        public void ConvertEmptyDateStringReturnsNull()
        {
            var res = String2DateTime.Convert(string.Empty);
            Assert.IsNull(res);
        }

        [Test]
        public void ConvertValidDateStringReturnsValidDate()
        {
            var res = String2DateTime.Convert("20151123");
            var expectedDate = new DateTime(2015, 11, 23);
            Assert.IsNotNull(res);
            Assert.AreEqual(expectedDate, res);
        }

        [Test]
        public void ConvertValidDateTimeStringsReturnsValidDateTime()
        {
            var res = String2DateTime.Convert("20151123", "100226");
            var expectedDate = new DateTime(2015, 11, 23, 10, 02, 26);
            Assert.IsNotNull(res);
            Assert.AreEqual(expectedDate, res);
        }

        [Test]
        public void ConvertDateWithZeroTimeReturnsValidDateTime()
        {
            var res = String2DateTime.Convert("20151123", "0");
            var expectedDate = new DateTime(2015, 11, 23, 0, 0, 0);
            Assert.IsNotNull(res);
            Assert.AreEqual(expectedDate, res);
        }

    }
}
