namespace BackTesting.Model.MarketData
{
    using System;

    public class Bar
    {
        public DateTime DateTime { get; set; }
        public string Symbol { get; set; }
        public string Period { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public long Volume { get; set; }

        public override string ToString()
        {
            return string.Format($"{this.DateTime} S={this.Symbol} P={this.Period} O={this.Open} H={this.High} L={this.Low} C={this.Close} V={this.Volume}");
        }
    }
}
