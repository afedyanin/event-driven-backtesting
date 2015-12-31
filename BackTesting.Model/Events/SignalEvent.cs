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
        public decimal Strength { get; private set; }

        /// <summary>
        /// Initialises the SignalEvent.
        /// </summary>
        /// <param name="symbol">The ticker symbol, e.g. 'GOOG'</param>
        /// <param name="timeStamp">The timestamp at which the signal was generated.</param>
        /// <param name="signalType">'LONG' or 'SHORT'</param>
        /// <param name="strength">strength of signal</param>
        public SignalEvent(string symbol, DateTime timeStamp, SignalType signalType, decimal strength = 0.1m)
        {
            this.Symbol = symbol;
            this.TimeStamp = timeStamp;
            this.SignalType = signalType;
            this.Strength = strength;
        }
    }
}
