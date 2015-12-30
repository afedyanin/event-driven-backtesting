namespace BackTesting.Model.DataHandlers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using BackTesting.Model.Utils;
    using Deedle;
    using Events;

    /// <summary>
    /// HistoricCSVDataHandler is designed to read CSV files for
    /// each requested symbol from disk and provide an interface
    /// to obtain the "latest" bar in a manner identical to a live
    /// trading interface. 
    /// </summary>
    public class HistoricCsvDataHandler : DataHandlerBase
    {
        private readonly IEventBus eventBus;
        private readonly string csvDirectory;
        private readonly string[] symbolList;

        private IDictionary<string, Frame<DateTime, string>> marketDataDictionary;

        public bool ContinueBacktest { get; set; }

        /// <summary>
        /// Initialises the historic data handler by requesting the location of the CSV files and a list of symbols.
        /// It will be assumed that all files are of the form 'symbol.csv', where symbol is a string in the list.
        /// </summary>
        /// <param name="eventBus">The Event Queue</param>
        /// <param name="csvDirectory">Absolute directory path to the CSV files</param>
        /// <param name="symbolList">A list of symbol strings</param>
        public HistoricCsvDataHandler(IEventBus eventBus, string csvDirectory, string[] symbolList)
        {
            this.eventBus = eventBus;
            this.csvDirectory = csvDirectory;
            this.symbolList = symbolList;

            this.marketDataDictionary = new Dictionary<string, Frame<DateTime, string>>();
            this.OpenConvertCsvFiles();
            this.ContinueBacktest = true;
        }

        public Frame<DateTime, string> GetAllBarsBySymbol(string symbol)
        {
            return this.marketDataDictionary[symbol];
        }

        public override void GetLatestBars(string symbol, int n = 1)
        {
        }

        public override void UpdateBars()
        {
            throw new System.NotImplementedException();
        }

        public void GetNewBar(string symbol)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Opens the CSV files from the data directory, converting them into pandas DataFrames within a symbol dictionary.
        /// For this handler it will be assumed that the data is taken from DTN IQFeed. Thus its format will be respected.
        /// </summary>
        private void OpenConvertCsvFiles()
        {
            IList<DateTime> rowKeys = null;

            foreach (var symbol in this.symbolList)
            {
                var dataFrame = String2TimeSeries.Convert(this.LoadDataFrame(symbol));
                this.marketDataDictionary.Add(symbol, dataFrame);
                rowKeys = this.UnionRowKeys(rowKeys, dataFrame.RowKeys).ToList();
                this.marketDataDictionary = this.ReindexDataFrames(this.marketDataDictionary, rowKeys);
            }
        }

        private Frame<int, string> LoadDataFrame(string symbol)
        {
            var csvPath = Path.Combine(this.csvDirectory, symbol + ".csv");
            var frame = Csv2Frame.LoadFromFile(csvPath);
            return frame;
        }

        private IEnumerable<DateTime> UnionRowKeys(IEnumerable<DateTime> source1, IEnumerable<DateTime> source2)
        {
            return source1?.Union(source2) ?? source2;
        }

        private Dictionary<string, Frame<DateTime, string>> ReindexDataFrames(IDictionary<string, Frame<DateTime, string>>  source, IList<DateTime> rowKeys)
        {
            var res = new Dictionary<string, Frame<DateTime, string>>();

            foreach (var key in source.Keys)
            {
                var reindexedFrame = source[key].RealignRows(rowKeys).SortRowsByKey();
                res.Add(key, reindexedFrame);
            }

            return res;
        }
    }
}
