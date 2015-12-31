namespace BackTesting.Model.DataHandlers
{
    using System;
    using BackTesting.Model.Entities;
    using Deedle;
    using Events;

    /// <summary>
    /// HistoricCSVDataHandler is designed to read CSV files for
    /// each requested symbol from disk and provide an interface
    /// to obtain the "latest" bar in a manner identical to a live
    /// trading interface. 
    /// </summary>
    public class HistoricDataHandler : DataHandlerBase
    {
        private readonly IEventBus eventBus;
        private readonly IMarketData marketData;

        public bool ContinueBacktest { get; set; }

        public HistoricDataHandler(IEventBus eventBus, IMarketData marketData)
        {
            this.eventBus = eventBus;
            this.marketData = marketData;
            this.ContinueBacktest = true;
        }

        // TODO: remove it
        public Frame<DateTime, string> GetAllBars(string symbol)
        {
            return this.marketData.GetBars(symbol);
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
    }
}
