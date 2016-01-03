namespace BackTesting.Model.Strategies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BackTesting.Model.DataHandlers;
    using BackTesting.Model.Events;

    /// <summary>
    /// This is an extremely simple strategy that goes LONG all of the 
    /// symbols as soon as a bar is received. It will never exit a position.
    /// It is primarily used as a testing mechanism for the Strategy class
    /// as well as a benchmark upon which to compare other strategies.
    /// </summary>
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

        /// <summary>
        /// For "Buy and Hold" we generate a single signal per symbol
        /// and then no additional signals.This means we are
        /// constantly long the market from the date of strategy
        /// initialisation.
        /// </summary>
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
