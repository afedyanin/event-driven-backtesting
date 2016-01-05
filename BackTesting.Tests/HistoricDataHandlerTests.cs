namespace BackTesting.Tests
{
    using System;
    using BackTesting.Model.DataHandlers;
    using BackTesting.Model.Events;
    using BackTesting.Model.MarketData;
    using BackTesting.Model.Utils;
    using NUnit.Framework;

    [TestFixture]
    public class HistoricDataHandlerTests
    {
        [Test]
        public void CanGetLastBarFromCsvMarketData()
        {
            var dataSource = CsvDataSource.CreateFormStrings(Mother.GenericCsvData);
            var marketData = new ComposedMarketData(dataSource.Bars);
            var eventBus = new QueuedEventBus();

            var handler = new HistoricDataHandler(eventBus, marketData);

            Console.WriteLine("Total rows = {0}", marketData.RowKeys.Count);

            var i = 0;
            while (handler.ContinueBacktest)
            {
                handler.Update();
                i++;

                var sber = handler.GetLast(Symbols.Sber);
                var vtbr = handler.GetLast(Symbols.Vtbr);

                Console.WriteLine($"Iteration {i}");

                if (sber == null)
                {
                    Console.WriteLine($"{handler.CurrentTime} => SBER is NULL");
                }
                else
                {
                    sber.Print();
                }
                if (vtbr == null)
                {
                    Console.WriteLine($"{handler.CurrentTime} => VTBR is NULL");
                }
                else
                {
                    vtbr.Print();
                }
                Console.WriteLine("------------------");
            }
        }
    }
}
