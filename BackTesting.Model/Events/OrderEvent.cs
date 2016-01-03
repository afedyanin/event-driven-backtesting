namespace BackTesting.Model.Events
{
    using System;

    /// <summary>
    /// Handles the event of sending an Order to an execution system.
    /// The order contains a symbol(e.g.GOOG), a type(market or limit),
    /// quantity and a direction.
    /// </summary>
    public class OrderEvent : EventBase
    {
        public override EventType EventType => EventType.Order;

        public string Symbol { get; private set; }
        public OrderType OrderType { get; private set; }
        public int Quantity { get; private set; }
        public TransactionDirection OrderDirection { get; private set; }
        public DateTime OrderTime { get; private set; }

        /// <summary>
        /// Initialises the order type, setting whether it is
        /// a Market order('MKT') or Limit order('LMT'), has
        /// a quantity(integral) and its direction('BUY' or 'SELL').
        /// </summary>
        /// <param name="symbol">The instrument to trade</param>
        /// <param name="orderType">Market or Limit</param>
        /// <param name="quantity">Non-negative integer for quantity</param>
        /// <param name="orderDirection">'BUY' or 'SELL' for long or short</param>
        /// <param name="orderTime">order timestamp</param>
        public OrderEvent(string symbol, OrderType orderType, int quantity, TransactionDirection orderDirection, DateTime orderTime)
        {
            this.Symbol = symbol;
            this.OrderType = orderType;
            this.Quantity = quantity;
            this.OrderDirection = orderDirection;
            this.OrderTime = orderTime;
        }

        /// <summary>
        /// Outputs the values within the Order.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format($"Order: Symbol={this.Symbol}, Type={this.OrderType}, Quantity={this.Quantity}, Direction={this.OrderDirection}, Time={this.OrderTime}");
        }
    }
}
