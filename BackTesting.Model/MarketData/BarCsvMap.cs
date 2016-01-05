namespace BackTesting.Model.MarketData
{
    using BackTesting.Model.Utils;
    using CsvHelper.Configuration;

    public sealed class BarCsvMap : CsvClassMap<Bar>
    {
        // <TICKER>,<PER>,<DATE>,<TIME>,<OPEN>,<HIGH>,<LOW>,<CLOSE>,<VOL>
        public BarCsvMap()
        {
            Map(m => m.DateTime).ConvertUsing(row => String2DateTime.Convert(row.GetField<string>("<DATE>"), row.GetField<string>("<TIME>")));
            Map(m => m.Symbol).Name("<TICKER>");
            Map(m => m.Period).Name("<PER>");
            Map(m => m.Open).Name("<OPEN>");
            Map(m => m.High).Name("<HIGH>");
            Map(m => m.Low).Name("<LOW>");
            Map(m => m.Close).Name("<CLOSE>");
            Map(m => m.Volume).Name("<VOL>");
        }
    }
}
