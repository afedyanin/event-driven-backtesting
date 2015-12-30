namespace ConsoleApp
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Text;
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

            var csvString = @"
<TICKER>,<PER>,<DATE>,<TIME>,<OPEN>,<HIGH>,<LOW>,<CLOSE>,<VOL>
SBER,1,20151123,100100,106.8500000,106.8600000,106.1400000,106.2900000,655780
SBER,1,20151123,100200,106.2700000,106.3300000,106.1400000,106.2600000,548880
SBER,1,20151123,100300,106.2600000,106.2600000,105.9300000,106.0500000,1028870
SBER,1,20151123,100400,106.1100000,106.3000000,106.0500000,106.2800000,371940
SBER,1,20151123,100500,106.2600000,106.2800000,106.1300000,106.2200000,478220
SBER,1,20151123,100600,106.2100000,106.2300000,106.0200000,106.1100000,281740
SBER,1,20151123,100700,106.1500000,106.3000000,106.0500000,106.2600000,138440
SBER,1,20151123,100800,106.2900000,106.5200000,106.2400000,106.4000000,384210
SBER,1,20151123,100900,106.3900000,106.4500000,106.3000000,106.4500000,218000
SBER,1,20151123,101000,106.4700000,106.4700000,106.3400000,106.4100000,122320
";

            var dataHandler = new HistoricCsvDataHandler(null, "Data", new[] { "sber", "vtbr" });
            var sber = dataHandler.GetAllBarsBySymbol("sber");
            var vtbr = dataHandler.GetAllBarsBySymbol("vtbr");
            vtbr.Print();
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

            var dataHandler = new HistoricCsvDataHandler(null, "Data", new [] {"sber", "vtbr"});
            var sber = dataHandler.GetAllBarsBySymbol("sber");
            var vtbr = dataHandler.GetAllBarsBySymbol("vtbr");
            vtbr.Print();
            sber.Print();

        //----------------------------------
                var oe = new OrderEvent("SBER", OrderType.Market, 10, Direction.Buy);
                Console.WriteLine(oe);
                    Console.ReadLine();

        // ---------------------------------

            // var sber = Csv2Frame.LoadFromFile("Data/sber.csv");
            var sber2 = Csv2Frame.LoadFromString(csvString);
            sber2.Print();

        */
        #endregion
    }
}
