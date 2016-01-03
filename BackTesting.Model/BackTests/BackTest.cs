namespace BackTesting.Model.BackTests
{
    using System;
    using System.Threading;
    using BackTesting.Model.DataHandlers;
    using BackTesting.Model.Events;
    using BackTesting.Model.ExecutionHandlers;
    using BackTesting.Model.Portfolio;
    using BackTesting.Model.Strategies;
    using BackTesting.Model.SummaryStatistics;
    using Deedle;

    public class BackTest
    {
        private readonly int heartBeatMilliseconds = 100;

        private readonly IEventBus eventBus;
        private readonly IDataHandler bars;
        private readonly IStrategy strategy;
        private readonly IPortfolio portfolio;
        private readonly IExecutionHandler executionHandler;
        private readonly ISummaryStatistics stats;

        public BackTest(
            IEventBus eventBus,
            IDataHandler bars, 
            IStrategy strategy, 
            IPortfolio portfolio, 
            IExecutionHandler executionHandler,
            ISummaryStatistics stats)
        {
            this.eventBus = eventBus;
            this.bars = bars;
            this.strategy = strategy;
            this.portfolio = portfolio;
            this.executionHandler = executionHandler;
            this.stats = stats;
        }

        public void SimulateTrading()
        {
            this.Run();
            this.OutputPerformance();
        }

        private void Run()
        {
            int i = 0;

            while (true)
            {
                i++;
                Console.WriteLine("Iteration {0}", i);
                if (this.bars.ContinueBacktest)
                {
                    this.bars.Update();
                }
                else
                {
                    break;
                }

                while (true)
                {
                    var evt = this.eventBus.Get();

                    if (evt == null)
                    {
                        break;
                    }

                    switch (evt.EventType)
                    {
                        case EventType.Market:
                            var mEvt = (MarketEvent) evt;
                            Console.WriteLine("Market event => {0}", mEvt.CurrentTime);
                            this.strategy.CalculateSignals();
                            this.portfolio.UpdateTimeIndex(mEvt);
                            break;
                        case EventType.Signal:
                            var signal = (SignalEvent)evt;
                            this.stats.UpdateSignalHistory(signal);
                            this.portfolio.UpdateSignal(signal);
                            break;
                        case EventType.Order:
                            var order = (OrderEvent)evt;
                            this.stats.UpdateOrderHistory(order);
                            this.executionHandler.ExecuteOrder(order);
                            break;
                        case EventType.Fill:
                            var fill = (FillEvent)evt;
                            this.stats.UpdateFillHistory(fill);
                            this.portfolio.UpdateFill(fill);
                            break;
                        default:
                            // TODO: Log undefined event
                            continue;
                    }
                }

                Thread.Sleep(this.heartBeatMilliseconds);
            }
        }

        private void OutputPerformance()
        {
            Console.WriteLine("\nCreating summary stats ...");

            Console.WriteLine("---------------------------");
            foreach (var signal in this.stats.SignalHistory)
            {
                Console.WriteLine(signal);
            }
            Console.WriteLine("---------------------------");
            foreach (var order in this.stats.OrderHistory)
            {
                Console.WriteLine(order);
            }
            Console.WriteLine("---------------------------");
            foreach (var fill in this.stats.FillHistory)
            {
                Console.WriteLine(fill);
            }

            Console.WriteLine("---------------------------");
            Console.WriteLine("\nHoldings");
            this.portfolio.GetHoldingHistory().Print();

            Console.WriteLine("---------------------------");
        }
    }
}
