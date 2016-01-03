namespace BackTesting.Model.Events
{
    using System;

    public class OrderEvent : Event
    {
        public override EventType EventType => EventType.Order;

        public string Symbol { get; private set; }
        public OrderType OrderType { get; private set; }
        public int Quantity { get; private set; }
        public TransactionDirection OrderDirection { get; private set; }
        public DateTime OrderTime { get; private set; }

        public OrderEvent(string symbol, OrderType orderType, int quantity, TransactionDirection orderDirection, DateTime orderTime)
        {
            this.Symbol = symbol;
            this.OrderType = orderType;
            this.Quantity = quantity;
            this.OrderDirection = orderDirection;
            this.OrderTime = orderTime;
        }

        public override string ToString()
        {
            return string.Format($"Order: {this.OrderTime} {this.Symbol} {this.OrderDirection} {this.OrderType} Qty={this.Quantity}");
        }
    }
}
