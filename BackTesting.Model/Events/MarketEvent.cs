namespace BackTesting.Model.Events
{
    /// <summary>
    /// Handles the event of receiving a new market update with corresponding bars.
    /// </summary>
    public class MarketEvent : EventBase
    {
        public override EventType EventType => EventType.Market;
    }
}
