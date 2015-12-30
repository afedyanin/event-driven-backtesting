namespace ConsoleApp
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using BackTesting.Model;
    using BackTesting.Model.DataHandlers;
    using BackTesting.Model.Events;
    using BackTesting.Model.Utils;
    using Deedle;

    class Program
    {
        private const int CONST_ScreenWidth = 150;
        private const int CONST_ScreenHeight = 40;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");

            SetupScreen();

            var dataHandler = new HistoricCsvDataHandler(null, "Data", new [] {"sber", "vtbr"});

            var sber = dataHandler.GetAllBarsBySymbol("sber");
            var vtbr = dataHandler.GetAllBarsBySymbol("vtbr");

            vtbr.Print();
            Console.ReadLine();

            sber.Print();
            Console.ReadLine();
        }

        public static void SetupScreen()
        {
            Console.WindowWidth = CONST_ScreenWidth;
            Console.BufferWidth = CONST_ScreenWidth;

            Console.WindowHeight = CONST_ScreenHeight;
            Console.BufferHeight = CONST_ScreenHeight;
        }

        #region Old stuff
        /*
                var oe = new OrderEvent("SBER", OrderType.Market, 10, Direction.Buy);
                Console.WriteLine(oe);
                    Console.ReadLine();
        */
        #endregion
    }
}
