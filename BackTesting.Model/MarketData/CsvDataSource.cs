namespace BackTesting.Model.MarketData
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using BackTesting.Model.Utils;

    public class CsvDataSource 
    {
        public IDictionary<string, IDictionary<DateTime, Bar>> Bars { get; }

        protected CsvDataSource()
        {
            this.Bars = new Dictionary<string, IDictionary<DateTime, Bar>>();
        }

        public static CsvDataSource CreateFromFiles(string csvDirectory, string[] symbolList)
        {
            var res = new CsvDataSource();

            foreach (var symbol in symbolList)
            {
                var csvPath = Path.Combine(csvDirectory, symbol + ".csv");
                res.AddOrReplaceBars(symbol, Csv2Frame.LoadBarsFromFile(csvPath));
            }

            return res;
        }

        public static CsvDataSource CreateFormStrings(IDictionary<string, string> csvData)
        {
            var res = new CsvDataSource();

            foreach (var kvp in csvData)
            {
                res.AddOrReplaceBars(kvp.Key, Csv2Frame.LoadBarsFromString(kvp.Value));
            }

            return res;
        }

        private void AddOrReplaceBars(string symbol, IDictionary<DateTime, Bar> bars)
        {
            if (this.Bars.ContainsKey(symbol))
            {
                this.Bars[symbol] = bars;
            }
            else
            {
                this.Bars.Add(symbol, bars);
            }
        }
    }
}
