namespace BackTesting.Model.Utils
{
    using System;
    using System.Collections.Generic;
    using BackTesting.Model.MarketData;

    public static class Extensions
    {
        public static void Print(this IDictionary<DateTime, Bar> bars)
        {
            foreach (var kvp in bars)
            {
                Console.WriteLine($"{kvp.Key} => {kvp.Value}");
            }
        }

        public static void Print(this Bar bar)
        {
            Console.WriteLine(bar);
        }
    }
}
