namespace BackTesting.Model.BackTests
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using BackTesting.Model.DataHandlers;
    using BackTesting.Model.Events;
    using BackTesting.Model.ExecutionHandlers;
    using BackTesting.Model.Portfolio;
    using BackTesting.Model.Strategies;
    using Deedle;

    public class BackTest
    {
        private readonly int heartBeatMilliseconds = 0;

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
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            this.Run();
            stopWatch.Stop();
            var ts = stopWatch.Elapsed;
            this.OutputPerformance();
            var elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("Run Time: " + elapsedTime);
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
                            if (iteration % 100 == 0)
                            {
                                Console.WriteLine($"{iteration} Market time: {mEvt.CurrentTime}");
                            }
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
            equityCurve.SaveCsv("equtycurve.csv", true);
            Console.WriteLine("---------------------------");
            Console.WriteLine($"Signals={this.signals} Orders={this.orders} Fills={this.fills}");
        }
    }
}
