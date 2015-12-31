namespace BackTesting.Model.DataSource.Csv
{
    using System.Collections.Generic;
    using Deedle;

    public abstract class CsvDataSource 
    {
        public IDictionary<string, Frame<int, string>> Frames { get; }

        protected CsvDataSource()
        {
            this.Frames = new Dictionary<string, Frame<int, string>>();
        }

        protected void AddOrReplace(string symbol, Frame<int, string> frame)
        {
            if (this.Frames.ContainsKey(symbol))
            {
                this.Frames[symbol] = frame;
            }
            else
            {
                this.Frames.Add(symbol, frame);
            }
        }
    }
}
