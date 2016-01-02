namespace BackTesting.Model.Portfolio
{
    using System;
    using BackTesting.Model.Events;
    using Deedle;

    /// <summary>
    /// The Portfolio class handles the positions and market
    /// value of all instruments at a resolution of a "bar",
    /// i.e.secondly, minutely, 5-min, 30-min, 60 min or EOD.
    /// </summary>
    public interface IPortfolio
    {
        void UpdateSignal(SignalEvent signal);
        void UpdateFill(FillEvent fill);
        void UpdateTimeIndex(MarketEvent market);
        Frame<DateTime, string> GetHoldingHistory();
    }
}
