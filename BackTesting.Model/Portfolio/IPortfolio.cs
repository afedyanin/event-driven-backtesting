namespace BackTesting.Model.Portfolio
{
    using System;
    using System.Collections.Generic;
    using BackTesting.Model.Events;

    public interface IPortfolio
    {
        void UpdateSignal(SignalEvent signal);
        void UpdateFill(FillEvent fill);
        void UpdateTimeIndex(MarketEvent market);
        IDictionary<DateTime, Holding> HoldingHistory { get; }
        IDictionary<DateTime, decimal> GetEquityCurve();
    }
}
