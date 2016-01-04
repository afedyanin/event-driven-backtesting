namespace BackTesting.Model.Strategies
{
    using System.Collections.Generic;
    using System.Linq;
    using BackTesting.Model.DataHandlers;
    using BackTesting.Model.Events;

    public class BuyAndHoldStrategy : IStrategy
    {
        private readonly IEventBus eventBus;
        private readonly IDataHandler bars;

        private readonly IDictionary<string, bool> bought;

        public BuyAndHoldStrategy(IEventBus eventBus, IDataHandler dataHandler)
        {
            this.eventBus = eventBus;
            this.bars = dataHandler;
            this.bought = this.CalculateInitialBought();
        }

        public void CalculateSignals()
        {
            if (!this.bars.CurrentTime.HasValue)
            {
                // not started yet
                return;
            }

            var currentTime = this.bars.CurrentTime.Value;

            foreach (var symbol in this.bars.Symbols)
            {
                if (this.bought[symbol])
                {
                    continue;
                }

                var lastBar = this.bars.GetLast(symbol);

                if (lastBar == null)
                {
                    // No market data
                    continue;
                }

                this.bought[symbol] = true;
                this.eventBus.Put(new SignalEvent(symbol, currentTime, SignalType.Long));
            }
        }

        private IDictionary<string, bool> CalculateInitialBought()
        {
            return this.bars.Symbols.ToDictionary(symbol => symbol, symbol => false);
        }
    }
}
