namespace BackTesting.Model.BackTests
{
    using System;
    using System.Threading;
    using BackTesting.Model.DataHandlers;
    using BackTesting.Model.Events;
    using BackTesting.Model.ExecutionHandlers;
    using BackTesting.Model.Portfolio;
    using BackTesting.Model.Strategies;

    public class BackTest
    {
        private readonly int heartBeatMilliseconds = 100;

        private int signals;
        private int orders;
        private int fills;

        private IEventBus eventBus;
        private DataHandlerBase dataHandler;
        private StrategyBase strategy;
        private PortfolioBase portfolio;
        private ExecutionHandlerBase executionHandler;

        public BackTest(
            IEventBus eventBus, 
            DataHandlerBase dataHandler, 
            StrategyBase strategy, 
            PortfolioBase portfolio, 
            ExecutionHandlerBase executionHandler)
        {
            this.eventBus = eventBus;
            this.dataHandler = dataHandler;
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
                if (this.dataHandler.ContinueBacktest)
                {
                    this.dataHandler.UpdateBars();
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
                            this.strategy.CalculateSignals();
                            this.portfolio.UpdateTimeIndex((MarketEvent)evt);
                            break;
                        case EventType.Signal:
                            this.signals++;
                            this.portfolio.UpdateSignal((SignalEvent)evt);
                            break;
                        case EventType.Order:
                            this.orders++;
                            this.executionHandler.ExecuteOrder((OrderEvent)evt);
                            break;
                        case EventType.Fill:
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
            Console.WriteLine("Creating equity curve ...");

            Console.WriteLine("Signals: {0}", this.signals);
            Console.WriteLine("Orders: {0}", this.orders);
            Console.WriteLine("Fills: {0}", this.fills);
        }
    }
}
