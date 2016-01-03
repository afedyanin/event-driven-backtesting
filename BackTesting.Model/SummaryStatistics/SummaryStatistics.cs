namespace BackTesting.Model.SummaryStatistics
{
    using System.Collections.Generic;
    using BackTesting.Model.Events;

    public class SummaryStatistics : ISummaryStatistics
    {
        public IList<SignalEvent> SignalHistory { get; }
        public IList<OrderEvent> OrderHistory { get; }
        public IList<FillEvent> FillHistory { get; }

        public SummaryStatistics()
        {
            this.SignalHistory = new List<SignalEvent>();
            this.OrderHistory = new List<OrderEvent>();
            this.FillHistory = new List<FillEvent>();
        }

        public void UpdateOrderHistory(OrderEvent order)
        {
            if (order == null)
            {
                return;
            }

            this.OrderHistory.Add(order);
        }
        public void UpdateFillHistory(FillEvent fill)
        {
            if (fill == null)
            {
                return;
            }

            this.FillHistory.Add(fill);
        }
        public void UpdateSignalHistory(SignalEvent signal)
        {
            if (signal == null)
            {
                return;
            }

            this.SignalHistory.Add(signal);
        }
    }
}
