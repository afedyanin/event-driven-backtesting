namespace BackTesting.Model.BackTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using BackTesting.Model.DataHandlers;
    using BackTesting.Model.Events;
    using BackTesting.Model.ExecutionHandlers;
    using BackTesting.Model.Portfolio;
    using BackTesting.Model.Strategies;
    using CsvHelper;

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

        private readonly Stopwatch stopWatch;

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
            this.stopWatch = new Stopwatch();
        }

        public void SimulateTrading()
        {
            this.Run();
            this.OutputPerformance();
        }

        private void Run()
        {
            this.stopWatch.Start();
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

            this.stopWatch.Stop();
        }

        private void OutputPerformance()
        {
            Console.WriteLine("\nCreating summary stats ...");
            Console.WriteLine("---------------------------");
            Console.WriteLine("Holdings");
            this.PrintHoldingHistory(this.portfolio.HoldingHistory);
            Console.WriteLine("---------------------------");
            Console.WriteLine("Equity");
            var equityCurve = this.portfolio.GetEquityCurve();
            this.PrintEquityCurve(equityCurve);
            this.SaveAsCsv(equityCurve, "equtycurve.csv");
            Console.WriteLine("---------------------------");
            Console.WriteLine($"Signals={this.signals} Orders={this.orders} Fills={this.fills}");

            var ts = this.stopWatch.Elapsed;
            Console.WriteLine($"Run Time: {ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}");

        }

        private void PrintHoldingHistory(IDictionary<DateTime, Holding> holdingHistory)
        {
            foreach (var key in holdingHistory.Keys.Take(15))
            {
                Console.WriteLine(holdingHistory[key]);
            }

            Console.WriteLine("......................................");

            foreach (var key in holdingHistory.Keys.OrderByDescending(k => k).Take(15))
            {
                Console.WriteLine(holdingHistory[key]);
            }
        }

        private void PrintEquityCurve(IDictionary<DateTime, decimal> equityCurve)
        {
            foreach (var key in equityCurve.Keys.Take(15))
            {
                Console.WriteLine($"{key} {equityCurve[key]}");
            }

            Console.WriteLine("......................................");

            foreach (var key in equityCurve.Keys.OrderByDescending(k => k).Take(15))
            {
                Console.WriteLine($"{key} {equityCurve[key]}");
            }
        }

        private void SaveAsCsv(IDictionary<DateTime, decimal> equityCurve, string fileName)
        {
            using (var tw = new StreamWriter(fileName))
            {
                var csvWriter = new CsvWriter(tw);

                foreach (var kvp in equityCurve)
                {
                    csvWriter.WriteRecord(new { DateTime = kvp.Key, Equity = kvp.Value });
                }
            }
        }
    }
}
