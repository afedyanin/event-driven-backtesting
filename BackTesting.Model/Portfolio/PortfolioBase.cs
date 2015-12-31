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
    public abstract class PortfolioBase
    {
        public abstract void UpdateSignal(SignalEvent signal);
        public abstract void UpdateFill(FillEvent fill);
        public abstract void UpdateTimeIndex(MarketEvent market);

        public abstract Frame<DateTime, string> GetPositionHistory();
        public abstract Frame<DateTime, string> GetHoldingHistory();
    }
}
