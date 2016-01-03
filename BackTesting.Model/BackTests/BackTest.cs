namespace BackTesting.Model.BackTests
{
    using System;
    using System.Threading;
    using BackTesting.Model.DataHandlers;
    using BackTesting.Model.Events;
    using BackTesting.Model.ExecutionHandlers;
    using BackTesting.Model.Portfolio;
    using BackTesting.Model.Strategies;
    using Deedle;

    public class BackTest
    {
        private readonly int heartBeatMilliseconds = 100;

        private int signals;
        private int orders;
        private int fills;

        private readonly IEventBus eventBus;
        private readonly IDataHandler bars;
        private readonly IStrategy strategy;
        private readonly IPortfolio portfolio;
        private readonly IExecutionHandler executionHandler;

        public BackTest(
            IEventBus eventBus,
            IDataHandler bars, 
            IStrategy strategy, 
            IPortfolio portfolio, 
            IExecutionHandler executionHandler)
        {
            this.eventBus = eventBus;
            this.bars = bars;
            this.strategy = strategy;
            this.portfolio = portfolio;
            this.executionHandler = executionHandler;

            this.signals = 0;
            this.orders = 0;
            this.fills = 0;
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
                            // Console.WriteLine("Signal event");
                            this.signals++;
                            this.portfolio.UpdateSignal((SignalEvent)evt);
                            break;
                        case EventType.Order:
                            // Console.WriteLine("Order event");
                            this.orders++;
                            this.executionHandler.ExecuteOrder((OrderEvent)evt);
                            break;
                        case EventType.Fill:
                            // Console.WriteLine("Fill event");
                            this.fills++;
                            this.portfolio.UpdateFill((FillEvent)evt);
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
            Console.WriteLine("Creating summary stats ...");

            var res = this.portfolio.GetHoldingHistory();

            Console.WriteLine("---------------------------");
            res.Print();

            Console.WriteLine("---------------------------");
            Console.WriteLine("Signals: {0}", this.signals);
            Console.WriteLine("Orders: {0}", this.orders);
            Console.WriteLine("Fills: {0}", this.fills);
        }
    }
}
