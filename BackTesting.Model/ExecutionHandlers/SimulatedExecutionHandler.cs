namespace BackTesting.Model.ExecutionHandlers
{
    using System.Globalization;
    using BackTesting.Model.Events;

    /// <summary>
    /// The simulated execution handler simply converts all order
    /// objects into their equivalent fill objects automatically
    /// without latency, slippage or fill-ratio issues.
    /// 
    /// This allows a straightforward "first go" test of any strategy,
    /// before implementation with a more sophisticated execution
    /// handler.
    /// </summary>
    public class SimulatedExecutionHandler : IExecutionHandler
    {
        private const int CONST_ExecutionDelaySeconds = 5;

        private readonly IEventBus eventBus;

        public SimulatedExecutionHandler(IEventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        /// <summary>
        /// Simply converts Order objects into Fill objects naively,
        /// i.e.without any latency, slippage or fill ratio problems.
        /// </summary>
        /// <param name="orderEvent"></param>
        public void ExecuteOrder(OrderEvent orderEvent)
        {
            // Simulate order execution delay
            var dateTime = orderEvent.OrderTime.AddSeconds(CONST_ExecutionDelaySeconds);

            var fillEvent = new FillEvent(
                dateTime.ToString(CultureInfo.InvariantCulture), 
                orderEvent.Symbol, 
                "ARCA", 
                orderEvent.Quantity, 
                orderEvent.OrderDirection, 
                decimal.Zero);

            this.eventBus.Put(fillEvent);
        }
    }
}
