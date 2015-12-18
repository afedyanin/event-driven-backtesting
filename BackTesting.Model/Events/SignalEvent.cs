namespace BackTesting.Model.Events
{
    using System;

    /// <summary>
    /// Handles the event of sending a Signal from a Strategy object.
    /// This is received by a Portfolio object and acted upon.
    /// </summary>
    public class SignalEvent : EventBase
    {
        public override EventType EventType => EventType.Signal;

        public string Symbol { get; private set; }
        public DateTime TimeStamp { get; private set; }
        public SignalType SignalType { get; private set; }

        /// <summary>
        /// Initialises the SignalEvent.
        /// </summary>
        /// <param name="symbol">The ticker symbol, e.g. 'GOOG'</param>
        /// <param name="timeStamp">The timestamp at which the signal was generated.</param>
        /// <param name="signalType">'LONG' or 'SHORT'</param>
        public SignalEvent(string symbol, DateTime timeStamp, SignalType signalType)
        {
            this.Symbol = symbol;
            this.TimeStamp = timeStamp;
            this.SignalType = signalType;
        }
    }
}
