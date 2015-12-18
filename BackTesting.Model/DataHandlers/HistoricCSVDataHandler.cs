namespace BackTesting.Model.DataHandlers
{
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

            this.ContinueBacktest = true;
            this.OpenConvertCsvFiles();
        }

        public override void GetLatestBars(string symbol, int n = 1)
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }
    }
}
