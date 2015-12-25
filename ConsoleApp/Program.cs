namespace ConsoleApp
{
    using System;
    using System.ComponentModel;
    using BackTesting.Model;
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

            Console.WindowWidth = CONST_ScreenWidth;
            Console.BufferWidth = CONST_ScreenWidth;

            Console.WindowHeight = CONST_ScreenHeight;
            Console.BufferHeight = CONST_ScreenHeight;

            var sber = ReindexByDateTime(Frame.ReadCsv("Data\\SBER_151123_151221.csv"));
            sber.Print();

            Console.ReadLine();
        }

        public static Frame<DateTime, string> ReindexByDateTime(
            Frame<int, string> frame, 
            string dateColumnName = "<DATE>", 
            string timeColumnName = "<TIME>", 
            string dateTimeColumnName = "DateTime")
        {
            var dtSeries = frame.Rows.Select(kvp => DateTimeStringConverter.Convert(
                            kvp.Value.GetAs<string>(dateColumnName),
                            kvp.Value.GetAs<string>(timeColumnName)));

            frame.AddColumn(dateTimeColumnName, dtSeries);
            frame.DropColumn(dateColumnName);
            frame.DropColumn(timeColumnName);

            return frame.IndexRows<DateTime>(dateTimeColumnName).SortRowsByKey();
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
