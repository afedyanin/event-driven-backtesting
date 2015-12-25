namespace BackTesting.Model.Utils
{
    using System;
    using System.Globalization;

    public static class DateTimeStringConverter
    {
        private const string CONST_DateFormat = "yyyyMMdd";
        private const string CONST_DateTimeFormat = "yyyyMMdd HHmmss";

        public static DateTime? Convert(string date, string time = null)
        {
            if (string.IsNullOrEmpty(date))
            {
                return null;
            }

            if (string.IsNullOrEmpty(time))
            {
                return ConvertDate(date);
            }

            return ConvertDateTime(date, time);
        }

        private static DateTime? ConvertDate(string date)
        {
            DateTime res;
            var isConverted = DateTime.TryParseExact(date, CONST_DateFormat, null, DateTimeStyles.None, out res);
            return isConverted ? (DateTime?)res : null;
        }

        private static DateTime? ConvertDateTime(string date, string time)
        {
            var dateTime = $"{date.Trim()} {time.Trim()}";
            DateTime res;
            var isConverted = DateTime.TryParseExact(dateTime, CONST_DateTimeFormat, null, DateTimeStyles.None, out res);
            return isConverted ? (DateTime?)res : null;
        }
    }
}
