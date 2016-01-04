namespace BackTesting.Model.ExecutionHandlers
{
    using BackTesting.Model.DataHandlers;
    using BackTesting.Model.Events;
    using BackTesting.Model.MarketData;

    public class SimulatedExecutionHandler : IExecutionHandler
    {
        private const int CONST_ExecutionDelaySeconds = 5;

        private readonly IEventBus eventBus;
        private readonly IDataHandler bars;

        public SimulatedExecutionHandler(IEventBus eventBus, IDataHandler bars)
        {
            this.eventBus = eventBus;
            this.bars = bars;
        }

        public void ExecuteOrder(OrderEvent orderEvent)
        {
            // Simulate order execution delay
            var dateTime = orderEvent.OrderTime.AddSeconds(CONST_ExecutionDelaySeconds);

            var closePrice = this.bars.GetLastClosePrice(orderEvent.Symbol);
            var fillCost = closePrice * orderEvent.Quantity ?? decimal.Zero;

            var fillEvent = new FillEvent(
                dateTime, 
                orderEvent.Symbol, 
                "ARCA", 
                orderEvent.Quantity, 
                orderEvent.OrderDirection, 
                fillCost);

            this.eventBus.Put(fillEvent);
        }
    }
}
