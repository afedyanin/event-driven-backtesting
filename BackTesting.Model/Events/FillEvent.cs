namespace BackTesting.Model.Events
{
    using System;

    /// <summary>
    /// Encapsulates the notion of a Filled Order, as returned
    /// from a brokerage.Stores the quantity of an instrument
    /// actually filled and at what price. In addition, stores
    /// the commission of the trade from the brokerage.
    /// </summary>
    public class FillEvent : EventBase
    {
        public override EventType EventType => EventType.Fill;

        public string TimeIndex { get; private set; }
        public string Symbol { get; private set; }
        public string Exchange { get; private set; }
        public int Quantity { get; private set; }
        public TransactionDirection Direction { get; private set; }
        public decimal FillCost { get; private set; }
        public decimal Comission { get; private set; }

        /// <summary>
        /// Initialises the FillEvent object. Sets the symbol, exchange, quantity, direction, cost of fill and an optional commission.
        /// If commission is not provided, the Fill object will calculate it based on the trade size and Interactive Brokers fees.
        /// </summary>
        /// <param name="timeIndex">The bar-resolution when the order was filled</param>
        /// <param name="symbol">The instrument which was filled</param>
        /// <param name="exchange">The exchange where the order was filled</param>
        /// <param name="quantity">The filled quantity</param>
        /// <param name="direction">The direction of fill ('BUY' or 'SELL')</param>
        /// <param name="fillCost">The holdings value in dollars</param>
        /// <param name="comission">An optional commission sent from IB</param>
        public FillEvent(string timeIndex, string symbol, string exchange, int quantity, TransactionDirection direction, decimal fillCost, decimal comission = decimal.Zero)
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
    }
}
