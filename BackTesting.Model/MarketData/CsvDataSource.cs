namespace BackTesting.Model.MarketData
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using BackTesting.Model.Utils;
    using Deedle;

    public class CsvDataSource 
    {
        public IDictionary<string, Frame<DateTime, string>> Frames { get; }

        protected CsvDataSource()
        {
            this.Frames = new Dictionary<string, Frame<DateTime, string>>();
        }

        public static CsvDataSource CreateFromFiles(string csvDirectory, string[] symbolList)
        {
            var res = new CsvDataSource();

            foreach (var symbol in symbolList)
            {
                var csvPath = Path.Combine(csvDirectory, symbol + ".csv");
                res.AddOrReplace(symbol, Csv2Frame.LoadFromFile(csvPath));
            }

            return res;
        }

        public static CsvDataSource CreateFormStrings(IDictionary<string, string> csvData)
        {
            var res = new CsvDataSource();

            foreach (var kvp in csvData)
            {
                res.AddOrReplace(kvp.Key, Csv2Frame.LoadFromString(kvp.Value));
            }

            return res;
        }

        private void AddOrReplace(string symbol, Frame<int, string> frame)
        {
            var dateTimeFrame = String2TimeSeries.Convert(frame);

            if (this.Frames.ContainsKey(symbol))
            {
                this.Frames[symbol] = dateTimeFrame;
            }
            else
            {
                this.Frames.Add(symbol, dateTimeFrame);
            }
        }

    }
}
