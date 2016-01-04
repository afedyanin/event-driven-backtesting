namespace BackTesting.Model.Events
{
    using System;

    public class SignalEvent : Event
    {
        public override EventType EventType => EventType.Signal;

        public string Symbol { get; private set; }
        public DateTime TimeStamp { get; private set; }
        public SignalType SignalType { get; private set; }
        public decimal Strength { get; private set; }

        public SignalEvent(string symbol, DateTime timeStamp, SignalType signalType, decimal strength = 1m)
        {
            this.Symbol = symbol;
            this.TimeStamp = timeStamp;
            this.SignalType = signalType;
            this.Strength = strength;
        }

        public override string ToString()
        {
            return string.Format($"Signal: {this.TimeStamp} {this.Symbol} {this.SignalType} Strength={this.Strength}");
        }
    }
}
