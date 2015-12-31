namespace BackTesting.Model.Portfolio
{
    using BackTesting.Model.Events;

    /// <summary>
    /// The Portfolio class handles the positions and market
    /// value of all instruments at a resolution of a "bar",
    /// i.e.secondly, minutely, 5-min, 30-min, 60 min or EOD.
    /// </summary>
    public abstract class PortfolioBase
    {
        /// <summary>
        /// Acts on a SignalEvent to generate new orders 
        /// based on the portfolio logic.
        /// </summary>
        public abstract void UpdateSignal(SignalEvent signal);

        /// <summary>
        /// Updates the portfolio current positions and holdings
        /// from a FillEvent.
        /// </summary>
        public abstract void UpdateFill(FillEvent fill);
    }
}
