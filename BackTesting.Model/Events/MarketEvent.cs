namespace BackTesting.Model.Events
{
    using System;

    /// <summary>
    /// Handles the event of receiving a new market update with corresponding bars.
    /// </summary>
    public class MarketEvent : Event
    {
        public override EventType EventType => EventType.Market;

        public DateTime CurrentTime { get; }

        public MarketEvent(DateTime currentTime)
        {
            this.CurrentTime = currentTime;
        }
    }
}
