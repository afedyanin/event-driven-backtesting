namespace BackTesting.Model.Portfolio
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Holding
    {
        public DateTime DateTime { get; set; }
        public IDictionary<string, decimal> SymbolHoldings { get; set; }
        public decimal Comission { get; set; }
        public decimal Cash { get; set; }
        public decimal Total { get; set; }
        public decimal Change { get; set; }

        public decimal Returns { get; set; }
        public decimal EquityCurve { get; set; }

        public Holding()
        {
            this.SymbolHoldings = new Dictionary<string, decimal>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var sh in SymbolHoldings)
            {
                sb.AppendFormat($"{sh.Key}={sh.Value} ");
            }

            sb.AppendFormat($"Comission={this.Comission} Cash={this.Cash} Total={this.Total} Change={this.Change}");

            return sb.ToString();
        }
    }
}
