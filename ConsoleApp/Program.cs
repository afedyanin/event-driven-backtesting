namespace ConsoleApp
{
    using System;
    using BackTesting.Model.BackTests;
    using BackTesting.Model.DataHandlers;
    using BackTesting.Model.Events;
    using BackTesting.Model.ExecutionHandlers;
    using BackTesting.Model.MarketData;
    using BackTesting.Model.Portfolio;
    using BackTesting.Model.Strategies;

    class Program
    {
        private const int CONST_ScreenWidth = 150;
        private const int CONST_ScreenHeight = 40;
        private const int CONST_BufferHeight = CONST_ScreenHeight * 10;

        static void Main(string[] args)
        {
            Console.WriteLine("BackTest starting...");

            SetupScreen();
            DoMainBackTest();

            Console.WriteLine("\nBackTest has been finished. Press ENTER to exit.");
            Console.ReadLine();
        }

        private static void DoMainBackTest()
        {
            var eventBus = new QueuedEventBus();
            var dataSource = CsvDataSource.CreateFromFiles("Data\\Min1", new[] { Symbols.Sber, Symbols.Vtbr });
            var marketData = new ComposedMarketData(dataSource.Bars);
            var bars = new HistoricDataHandler(eventBus, marketData);
            var strategy = new BuyAndHoldStrategy(eventBus, bars);
            var executionHandler = new SimulatedExecutionHandler(eventBus, bars);
            var portfolio = new NaivePortfolio(eventBus, bars, 10000m);
            var backTest = new BackTest(eventBus, bars, strategy, portfolio, executionHandler);

            backTest.SimulateTrading();
        }

        private static void SetupScreen()
        {
            Console.WindowWidth = CONST_ScreenWidth;
            Console.BufferWidth = CONST_ScreenWidth;

            Console.WindowHeight = CONST_ScreenHeight;
            Console.BufferHeight = CONST_BufferHeight;
        }
    }
}
