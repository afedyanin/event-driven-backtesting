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
    public class NaivePortfolio : IPortfolio
    {
        private readonly IEventBus eventBus;
        private readonly IDataHandler bars;
        private readonly decimal initialCapital;

        private readonly IDictionary<DateTime, Series<string, decimal>> holdingHistory;
        private readonly IDictionary<string, int> currentPositions;
        private decimal currentComission;
        private decimal currentCash;

        public NaivePortfolio(IEventBus eventBus, IDataHandler bars, decimal initialCapital)
        {
            this.eventBus = eventBus;
            this.bars = bars;
            this.initialCapital = initialCapital;

            this.holdingHistory = new Dictionary<DateTime, Series<string, decimal>>();
            this.currentPositions = this.bars.Symbols.ToDictionary(symbol => symbol, qty => 0); ;
            this.currentComission = decimal.Zero;
            this.currentCash = this.initialCapital;
        }

        public Frame<DateTime, string> GetHoldingHistory()
        {
            return Frame.FromRows(this.holdingHistory);
        }

        public Frame<DateTime, string> GetEquityCurve()
        {
            var dict = new Dictionary<DateTime, Series<string,decimal>>();

            var prevTotal = decimal.Zero;
            var equity = decimal.Zero;

            foreach (var kvp in this.holdingHistory)
            {
                var total = kvp.Value.Get("Total");

                var returns = prevTotal != decimal.Zero 
                    ? ((total - prevTotal) / prevTotal) * 100 
                    : decimal.Zero;

                prevTotal = total;

                var change = this.initialCapital != decimal.Zero
                    ? ((total - this.initialCapital)/this.initialCapital)*100
                    : decimal.Zero;

                // TODO: Fix it
                equity = (1.0m + returns);

                var sb = new SeriesBuilder<string, decimal>();

                sb.Add("Change %", change);
                sb.Add("Returns", returns);
                sb.Add("Equity_Curve", equity);

                dict.Add(kvp.Key, sb.Series);
            }

            return Frame.FromRows(dict);
        }

        public void UpdateSignal(SignalEvent signal)
        {
            var orderEvent = this.GenerateNaiveOrder(signal);
            if (orderEvent != null)
            {
                this.eventBus.Put(orderEvent);
            }
        }

        public void UpdateFill(FillEvent fill)
        {
            var fillDir = GetNumericDirection(fill.Direction);
            var closePrice = GetLastClosePrice(fill.Symbol);

            if (closePrice == null)
            {
                throw new InvalidOperationException($"Cannot find last price for {fill.Symbol}");
            }

            var cost = fillDir * closePrice.Value * fill.Quantity;
            this.currentPositions[fill.Symbol] += fillDir * fill.Quantity;
            this.currentComission += fill.Comission;
            this.currentCash -= (cost + fill.Comission);
        }

        public void UpdateTimeIndex(MarketEvent market)
        {
            var sb = new SeriesBuilder<string, decimal>();

            var marketHoldings = this.bars.Symbols.ToDictionary(symbol => symbol, cost => decimal.Zero); 

            var cash = this.currentCash;
            var commision = this.currentComission;
            var total = this.currentCash;

            foreach (var symbol in this.bars.Symbols)
            {
                var qty = this.currentPositions[symbol];
                var closePrice = GetLastClosePrice(symbol);

                if (closePrice == null)
                {
                    if (qty != 0)
                    {
                        throw new InvalidOperationException($"Unknown close price for {symbol} with position qty={qty}.");
                    }

                    closePrice = decimal.Zero;
                }

                var marketValue = qty * closePrice.Value;
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
            var time = signal.TimeStamp;

            var marketQuantity = (int)Math.Floor(100*strength);
            var currentQuantity = (int)this.currentPositions[symbol];

            if (direction == SignalType.Long && currentQuantity == 0)
            {
                return new OrderEvent(symbol, OrderType.Market, marketQuantity, TransactionDirection.Buy, time);
            }
            if (direction == SignalType.Short && currentQuantity == 0)
            {
                return new OrderEvent(symbol, OrderType.Market, marketQuantity, TransactionDirection.Sell, time);
            }
            if (direction == SignalType.Exit && currentQuantity > 0)
            {
                return new OrderEvent(symbol, OrderType.Market, Math.Abs(currentQuantity), TransactionDirection.Sell, time);
            }
            if (direction == SignalType.Exit && currentQuantity < 0)
            {
                return new OrderEvent(symbol, OrderType.Market, Math.Abs(currentQuantity), TransactionDirection.Buy, time);
            }

            return null;
        }

        private decimal? GetLastClosePrice(string symbol)
        {
            var lastBar = this.bars.GetLast(symbol);
            return (decimal?) lastBar?["<CLOSE>"];
        }

        // TODO: Convert to extension
        private static int GetNumericDirection(TransactionDirection dir)
        {
            return dir == TransactionDirection.Buy ? 1 : dir == TransactionDirection.Sell ? -1 : 0;
        }
    }
}
