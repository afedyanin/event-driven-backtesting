namespace BackTesting.Tests
{
    using System;
    using Model.Utils;
    using NUnit.Framework;

    [TestFixture]
    public class DateTimeStringConverterTests
    {
        [Test]
        public void ConvertEmptyDateStringReturnsNull()
        {
            var res = DateTimeStringConverter.Convert(string.Empty);
            Assert.IsNull(res);
        }

        [Test]
        public void ConvertValidDateStringReturnsValidDate()
        {
            var res = DateTimeStringConverter.Convert("20151123");
            var expectedDate = new DateTime(2015, 11, 23);
            Assert.IsNotNull(res);
            Assert.AreEqual(expectedDate, res);
        }

        [Test]
        public void ConvertValidDateTimeStringsReturnsValidDateTime()
        {
            var res = DateTimeStringConverter.Convert("20151123", "100226");
            var expectedDate = new DateTime(2015, 11, 23, 10, 02, 26);
            Assert.IsNotNull(res);
            Assert.AreEqual(expectedDate, res);
        }
    }
}
