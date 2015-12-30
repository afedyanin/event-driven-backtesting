namespace BackTesting.Model.Utils
{
    using System;
    using System.Globalization;

    public static class String2DateTime
    {
        public static DateTime? Convert(string date, string time = null, string dateFormat = "yyyyMMdd", string timeFormat = "HHmmss")
        {
            if (string.IsNullOrEmpty(date))
            {
                return null;
            }

            var dateTimeFormat = $"{dateFormat.Trim()} {timeFormat.Trim()}";
            return string.IsNullOrEmpty(time) ? ConvertDate(date, dateFormat) : ConvertDateTime(date, time, dateTimeFormat);
        }

        private static DateTime? ConvertDate(string date, string dateFormat)
        {
            DateTime res;
            var isConverted = DateTime.TryParseExact(date, dateFormat, null, DateTimeStyles.None, out res);
            return isConverted ? (DateTime?)res : null;
        }

        private static DateTime? ConvertDateTime(string date, string time, string dateTimeFormat)
        {
            var dateTime = $"{date.Trim()} {time.Trim()}";
            DateTime res;
            var isConverted = DateTime.TryParseExact(dateTime, dateTimeFormat, null, DateTimeStyles.None, out res);
            return isConverted ? (DateTime?)res : null;
        }
    }
}
