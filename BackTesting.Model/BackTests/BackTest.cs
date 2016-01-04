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

        private readonly IEventBus eventBus;
        private readonly IDataHandler bars;
        private readonly IStrategy strategy;
        private readonly IPortfolio portfolio;
        private readonly IExecutionHandler executionHandler;

        private int signals = 0;
        private int orders = 0;
        private int fills = 0;

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
        }

        public void SimulateTrading()
        {
            this.Run();
            this.OutputPerformance();
        }

        private void Run()
        {
            var iteration = 0;
            while (true)
            {
                iteration++;
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
                            Console.WriteLine($"{iteration} Market time: {mEvt.CurrentTime}");
                            this.strategy.CalculateSignals();
                            this.portfolio.UpdateTimeIndex(mEvt);
                            break;
                        case EventType.Signal:
                            var signal = (SignalEvent)evt;
                            Console.WriteLine($" => {signal}");
                            this.portfolio.UpdateSignal(signal);
                            this.signals++;
                            break;
                        case EventType.Order:
                            var order = (OrderEvent)evt;
                            Console.WriteLine($" => {order}");
                            this.executionHandler.ExecuteOrder(order);
                            this.orders++;
                            break;
                        case EventType.Fill:
                            var fill = (FillEvent)evt;
                            Console.WriteLine($" => {fill}");
                            this.portfolio.UpdateFill(fill);
                            this.fills++;
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
            Console.WriteLine("Holdings");
            this.portfolio.GetHoldingHistory().Print();
            Console.WriteLine("---------------------------");
            Console.WriteLine("Equity");
            var equityCurve = this.portfolio.GetEquityCurve();
            equityCurve.Print();
            equityCurve.SaveCsv("equtycurve.csv");
            Console.WriteLine("---------------------------");
            Console.WriteLine($"Signals={this.signals} Orders={this.orders} Fills={this.fills}");
        }
    }
}
