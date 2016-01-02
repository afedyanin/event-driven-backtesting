namespace BackTesting.Model.Portfolio
{
    using System;
    using BackTesting.Model.DataHandlers;
    using BackTesting.Model.Events;
    using System.Collections.Generic;
    using System.Linq;
    using Deedle;

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

        private readonly IDictionary<DateTime, Series<string, decimal>> holdingHistory;
        private readonly IDictionary<string, int> currentPositions;
        private decimal currentComission;
        private decimal currentCash;

        public NaivePortfolio(IEventBus eventBus, DataHandlerBase bars, decimal initialCapital, DateTime startTime)
        {
            this.eventBus = eventBus;
            this.bars = bars;

            this.holdingHistory = new Dictionary<DateTime, Series<string, decimal>>();
            this.currentPositions = this.bars.Symbols.ToDictionary(symbol => symbol, qty => 0); ;
            this.currentComission = decimal.Zero;
            this.currentCash = initialCapital;
        }

        public override Frame<DateTime, string> GetHoldingHistory()
        {
            return Frame.FromRows(this.holdingHistory);
        }

        public override void UpdateSignal(SignalEvent signal)
        {
            var orderEvent = this.GenerateNaiveOrder(signal);
            if (orderEvent != null)
            {
                this.eventBus.Put(orderEvent);
            }
        }

        public override void UpdateFill(FillEvent fill)
        {
            var fillDir = GetNumericDirection(fill.Direction);
            var closePrice = GetLastClosePrice(fill.Symbol);
            var cost = fillDir * closePrice * fill.Quantity;

            this.currentPositions[fill.Symbol] += fillDir * fill.Quantity;
            this.currentComission += fill.Comission;
            this.currentCash -= (cost + fill.Comission);
        }

        public override void UpdateTimeIndex(MarketEvent market)
        {
            var sb = new SeriesBuilder<string, decimal>();

            var marketHoldings = this.bars.Symbols.ToDictionary(symbol => symbol, cost => decimal.Zero); 

            var cash = this.currentCash;
            var commision = this.currentComission;
            var total = this.currentCash;

            foreach (var symbol in marketHoldings.Keys)
            {
                var qty = this.currentPositions[symbol];
                var closePrice = GetLastClosePrice(symbol);
                var marketValue = qty * closePrice;

                marketHoldings[symbol] = marketValue;
                total += marketValue;
            }

            foreach (var kvp in marketHoldings)
            {
                sb.Add(kvp.Key, kvp.Value);
            }

            sb.Add("Comission", commision);
            sb.Add("Cash", cash);
            sb.Add("Total", total);

            var dateTime = market.CurrentTime;

            if (this.holdingHistory.ContainsKey(dateTime))
            {
                this.holdingHistory[dateTime] = sb.Series;
            }
            else
            {
                this.holdingHistory.Add(dateTime, sb.Series);
            }
        }

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
                return new OrderEvent(symbol, orderType, marketQuantity, TransactionDirection.Buy);
            }
            if (direction == SignalType.Short && currentQuantity == 0)
            {
                return new OrderEvent(symbol, orderType, marketQuantity, TransactionDirection.Sell);
            }
            if (direction == SignalType.Exit && currentQuantity > 0)
            {
                return new OrderEvent(symbol, orderType, Math.Abs(currentQuantity), TransactionDirection.Sell);
            }
            if (direction == SignalType.Exit && currentQuantity < 0)
            {
                return new OrderEvent(symbol, orderType, Math.Abs(currentQuantity), TransactionDirection.Buy);
            }

            return null;
        }

        private decimal GetLastClosePrice(string symbol)
        {
            var lastBar = this.bars.GetLast(symbol);
            var closePrice = (decimal)lastBar["<CLOSE>"];
            return closePrice;
        }

        // TODO: Convert to extension
        private static int GetNumericDirection(TransactionDirection dir)
        {
            return dir == TransactionDirection.Buy ? 1 : dir == TransactionDirection.Sell ? -1 : 0;
        }
    }
}
