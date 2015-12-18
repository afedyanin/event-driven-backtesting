namespace ConsoleApp
{
    using System;
    using BackTesting.Model;
    using BackTesting.Model.Events;

    class Program
    {
        static void Main(string[] args)
        {
            var oe = new OrderEvent("SBER", OrderType.Market, 10, Direction.Buy);
            Console.WriteLine(oe);
            Console.ReadLine();
        }
    }
}
