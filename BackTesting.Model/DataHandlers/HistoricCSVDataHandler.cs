namespace BackTesting.Model.DataHandlers
{
    using System;
    using BackTesting.Model.DataSource.Csv;
    using BackTesting.Model.Entities;
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
        private readonly ComposedMarketData marketData;

        public bool ContinueBacktest { get; set; }

        public HistoricCsvDataHandler(IEventBus eventBus, ICsvDataSource dataSource)
        {
            this.eventBus = eventBus;
            this.marketData = FillMarketData(dataSource);
            this.ContinueBacktest = true;
        }

        // TODO: remove it
        public Frame<DateTime, string> GetAllBarsBySymbol(string symbol)
        {
            return this.marketData.Get(symbol);
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

        private static ComposedMarketData FillMarketData(ICsvDataSource dataSource)
        {
            var mdata = new ComposedMarketData();
            foreach (var kvp in dataSource.CsvFrames)
            {
                mdata.Compose(kvp.Key, String2TimeSeries.Convert(kvp.Value));
            }

            return mdata;
        }
    }
}
