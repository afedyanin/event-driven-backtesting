namespace BackTesting.Model.Utils
{
    using System;
    using BackTesting.Model.MarketData;
    using Deedle;

    public static class String2TimeSeries
    {
        private const string CONST_IndexColumnName = "DateTimeKeys";

        public static Frame<DateTime, string> Convert(Frame<int, string> frame)
        {
            var res = ReplaceStringColumnsWithDateTime(frame, ColumnNames.Date, ColumnNames.Time, ColumnNames.DateTime);
            return ReindexRowsByDateTime(res);
        }

        private static Frame<int, string> ReplaceStringColumnsWithDateTime(
            Frame<int, string> frame,
            string stringDateColumnName,
            string stringTimeColumnName,
            string dateTimeColumnName)
        {
            var dtSeries = frame.Rows.Select(kvp => String2DateTime.Convert(
                            kvp.Value.GetAs<string>(stringDateColumnName),
                            kvp.Value.GetAs<string>(stringTimeColumnName)));

            frame.AddColumn(CONST_IndexColumnName, dtSeries);
            frame.AddColumn(dateTimeColumnName, dtSeries);
            frame.DropColumn(stringDateColumnName);
            frame.DropColumn(stringTimeColumnName);

            return frame;
        }

        private static Frame<DateTime, string> ReindexRowsByDateTime(Frame<int, string> frame)
        {
            return frame.IndexRows<DateTime>(CONST_IndexColumnName).SortRowsByKey();
        }
    }
}
