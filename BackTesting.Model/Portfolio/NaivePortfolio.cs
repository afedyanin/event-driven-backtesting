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

        private readonly IDictionary<string, int> currentPositions;
        private readonly IDictionary<string, decimal> currentHoldings;

        private decimal currentComission;
        private decimal currentCash;
        private decimal currentTotal;

        private readonly IDictionary<DateTime, Series<string, decimal>> positionHistory; 
        private readonly IDictionary<DateTime, Series<string, decimal>> holdingHistory; 

        public NaivePortfolio(IEventBus eventBus, DataHandlerBase bars, decimal initialCapital, DateTime startTime)
        {
            this.eventBus = eventBus;
            this.bars = bars;

            this.currentPositions = this.ConstructCurrentPositions();

            this.currentHoldings = this.ConstructCurrentHoldings();
            this.currentComission = decimal.Zero;
            this.currentCash = initialCapital;
            this.currentTotal = initialCapital;

            this.positionHistory = new Dictionary<DateTime, Series<string, decimal>>();
            this.AppendPositionHistory(startTime);

            this.holdingHistory = new Dictionary<DateTime, Series<string, decimal>>();
        }

        public override Frame<DateTime, string> GetPositionHistory()
        {
            return Frame.FromRows(this.positionHistory);
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
            this.UpdatePositionsFromFill(fill);
            this.UpdateHoldingsFromFill(fill);
        }

        public override void UpdateTimeIndex(MarketEvent market)
        {
            this.AppendPositionHistory(market.CurrentTime);
            this.AppendHoldingHistory(market.CurrentTime);
        }

        private void UpdatePositionsFromFill(FillEvent fill)
        {
            var fillDir = GetNumericDirection(fill.Direction);
            this.currentPositions[fill.Symbol] += fillDir*fill.Quantity;
        }

        private void UpdateHoldingsFromFill(FillEvent fill)
        {
            var fillDir = GetNumericDirection(fill.Direction);
            var closePrice = GetLastClosePrice(fill.Symbol);
            var cost = fillDir*closePrice*fill.Quantity;

            this.currentHoldings[fill.Symbol] += cost;
            this.currentComission += fill.Comission;
            this.currentCash -= (cost + fill.Comission);
            this.currentTotal -= (cost + fill.Comission);
        }

        private IDictionary<string, int> ConstructCurrentPositions()
        {
            return this.bars.Symbols.ToDictionary(symbol => symbol, qty => 0);
        }

        private void AppendPositionHistory(DateTime dateTime)
        {
            var sb = new SeriesBuilder<string, decimal>();

            foreach (var kvp in this.currentPositions)
            {
                sb.Add(kvp.Key, kvp.Value);
            }

            if (this.positionHistory.ContainsKey(dateTime))
            {
                this.positionHistory[dateTime] = sb.Series;
            }
            else
            {
                this.positionHistory.Add(dateTime, sb.Series);
            }
        }

        private IDictionary<string, decimal> ConstructCurrentHoldings()
        {
            return this.bars.Symbols.ToDictionary(symbol => symbol, cost => decimal.Zero);
        }

        private void AppendHoldingHistory(DateTime dateTime) 
        {
            var sb = new SeriesBuilder<string, decimal>();

            var marketHoldings = this.ConstructCurrentHoldings();
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
