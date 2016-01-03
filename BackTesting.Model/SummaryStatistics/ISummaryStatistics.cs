namespace BackTesting.Model.SummaryStatistics
{
    using System.Collections.Generic;
    using BackTesting.Model.Events;

    public interface ISummaryStatistics
    {
        IList<SignalEvent> SignalHistory { get; }
        void UpdateSignalHistory(SignalEvent signal);

        IList<OrderEvent> OrderHistory { get; }
        void UpdateOrderHistory(OrderEvent order);

        IList<FillEvent> FillHistory { get; }
        void UpdateFillHistory(FillEvent fill);
    }
}
