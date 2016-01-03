namespace BackTesting.Model.Events
{
    using System;

    public class FillEvent : Event
    {
        public override EventType EventType => EventType.Fill;

        public DateTime TimeIndex { get; private set; }
        public string Symbol { get; private set; }
        public string Exchange { get; private set; }
        public int Quantity { get; private set; }
        public TransactionDirection Direction { get; private set; }
        public decimal FillCost { get; private set; }
        public decimal Comission { get; private set; }

        public FillEvent(DateTime timeIndex, string symbol, string exchange, int quantity, TransactionDirection direction, decimal fillCost, decimal comission = decimal.Zero)
        {
            this.TimeIndex = timeIndex;
            this.Symbol = symbol;
            this.Exchange = exchange;
            this.Quantity = quantity;
            this.Direction = direction;
            this.FillCost = fillCost;
            this.Comission = comission == decimal.Zero ? this.CalculateIbComission() : comission;
        }

        /// <summary>
        /// Calculates the fees of trading based on an Interactive Brokers fee structure for API, in USD. 
        /// This does not include exchange or ECN fees.
        /// Based on "US API Directed Orders": https://www.interactivebrokers.com/en/index.php?f=commission&p=stocks2
        /// </summary>
        /// <returns></returns>
        private decimal CalculateIbComission()
        {
            var fullCost = this.Quantity <= 500 ? Math.Max(1.3m, 0.013m * this.Quantity) : Math.Max(1.3m, 0.008m * this.Quantity);
            return Math.Min(fullCost, (0.5m/100.0m * this.Quantity * this.FillCost));
        }

        public override string ToString()
        {
            return string.Format($"Fill: {this.TimeIndex} {this.Exchange} {this.Symbol} {this.Direction} Qty={this.Quantity} FillCost={this.FillCost} Comission={this.Comission}");
        }
    }
}
