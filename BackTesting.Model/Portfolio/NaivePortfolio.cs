namespace BackTesting.Model.Portfolio
{
    using System;
    using BackTesting.Model.DataHandlers;
    using BackTesting.Model.Entities;
    using BackTesting.Model.Events;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The NaivePortfolio object is designed to send orders to
    /// a brokerage object with a constant quantity size blindly,
    /// i.e.without any risk management or position sizing.It is
    /// used to test simpler strategies such as BuyAndHoldStrategy.
    /// </summary>
    public class NaivePortfolio : PortfolioBase
    {
        private readonly IEventBus eventBus;
        private readonly DataHandlerBase bars;

        private readonly IDictionary<string, int> currentPositions;
        private readonly IDictionary<string, decimal> currentHoldings;

        private decimal comission;
        private decimal cash;
        private decimal total;

        public NaivePortfolio(IEventBus eventBus, DataHandlerBase bars, decimal initialCapital)
        {
            this.eventBus = eventBus;
            this.bars = bars;
            this.currentPositions = this.ConstructCurrentPositions();
            this.currentHoldings = this.ConstructCurrentHoldings();

            this.comission = decimal.Zero;
            this.cash = initialCapital;
            this.total = initialCapital;
        }

        /// <summary>
        /// Acts on a SignalEvent to generate new orders 
        /// based on the portfolio logic.
        /// </summary>
        /// <param name="signal"></param>
        public override void UpdateSignal(SignalEvent signal)
        {
            var orderEvent = this.GenerateNaiveOrder(signal);
            if (orderEvent != null)
            {
                this.eventBus.Put(orderEvent);
            }
        }

        /// <summary>
        /// Updates the portfolio current positions and holdings 
        /// from a FillEvent.
        /// </summary>
        /// <param name="fill"></param>
        public override void UpdateFill(FillEvent fill)
        {
            this.UpdatePositionsFromFill(fill);
            this.UpdateHoldingsFromFill(fill);
        }

        /// <summary>
        /// Adds a new record to the positions matrix for the current 
        /// market data bar.This reflects the PREVIOUS bar, i.e.all
        /// current market data at this stage is known (OLHCVI).
        /// </summary>
        /// <param name="market"></param>
        public void UpdateTimeIndex(MarketEvent market)
        {
            
        }

        /// <summary>
        /// Takes a FilltEvent object and updates the position matrix
        /// to reflect the new position.
        /// </summary>
        /// <param name="fill">The FillEvent object to update the positions with.</param>
        private void UpdatePositionsFromFill(FillEvent fill)
        {
            var fillDir = GetNumericDirection(fill.Direction);
            this.currentPositions[fill.Symbol] += fillDir*fill.Quantity;
        }

        /// <summary>
        /// Takes a FillEvent object and updates the holdings matrix
        ///  to reflect the holdings value.
        /// </summary>
        /// <param name="fill">The FillEvent object to update the holdings with.</param>
        private void UpdateHoldingsFromFill(FillEvent fill)
        {
            var fillDir = GetNumericDirection(fill.Direction);
            var lastBar = this.bars.GetLatestBars(fill.Symbol).First();
            var closePrice = (decimal)lastBar["<CLOSE>"];
            var cost = fillDir*closePrice*fill.Quantity;
            this.currentHoldings[fill.Symbol] += cost;

            this.comission += fill.Comission;
            this.cash -= (cost + fill.Comission);
            this.total -= (cost + fill.Comission);
        }

        // TODO: Convert to extension
        private static int GetNumericDirection(Direction dir)
        {
            return dir == Direction.Buy ? 1 : dir == Direction.Sell ? -1 : 0;
        }

        /// <summary>
        /// Constructs the positions list using the start_date
        /// to determine when the time index will begin.
        /// </summary>
        /// <returns></returns>
        private IDictionary<string, int> ConstructCurrentPositions()
        {
            return this.bars.Symbols.ToDictionary(symbol => symbol, qty => 0);
        }

        private IDictionary<string, decimal> ConstructCurrentHoldings()
        {
            return this.bars.Symbols.ToDictionary(symbol => symbol, cost => decimal.Zero);
        }

        /// <summary>
        /// Simply transacts an OrderEvent object as a constant quantity
        /// sizing of the signal object, without risk management or
        /// position sizing considerations.
        /// </summary>
        /// <param name="signal">The SignalEvent signal information.</param>
        /// <returns></returns>
        private OrderEvent GenerateNaiveOrder(SignalEvent signal)
        {
            var symbol = signal.Symbol;
            var direction = signal.SignalType;
            var strength = signal.Strength;

            var marketQuantity = (int)Math.Floor(100*strength);
            var currentQuantity = (int)this.currentPositions[symbol];
            var orderType = OrderType.Market;

            if (direction == SignalType.Long && currentQuantity == 0)
            {
                return new OrderEvent(symbol, orderType, marketQuantity, Direction.Buy);
            }
            if (direction == SignalType.Short && currentQuantity == 0)
            {
                return new OrderEvent(symbol, orderType, marketQuantity, Direction.Sell);
            }
            if (direction == SignalType.Exit && currentQuantity > 0)
            {
                return new OrderEvent(symbol, orderType, Math.Abs(currentQuantity), Direction.Sell);
            }
            if (direction == SignalType.Exit && currentQuantity < 0)
            {
                return new OrderEvent(symbol, orderType, Math.Abs(currentQuantity), Direction.Buy);
            }

            return null;
        }
    }
}
