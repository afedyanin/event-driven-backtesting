namespace BackTesting.Model.DataSource.Csv
{
    using System.Collections.Generic;
    using BackTesting.Model.Entities;
    using Deedle;

    public abstract class CsvDataSource : ICsvDataSource
    {
        private readonly IDictionary<string, Frame<int, string>> csvFramesDictionary;

        public IDictionary<string, Frame<int, string>> CsvFrames => this.csvFramesDictionary;

        protected CsvDataSource()
        {
            this.csvFramesDictionary = new Dictionary<string, Frame<int, string>>();
        }

        protected void AddOrReplace(string symbol, Frame<int, string> frame)
        {
            if (this.csvFramesDictionary.ContainsKey(symbol))
            {
                this.csvFramesDictionary[symbol] = frame;
            }
            else
            {
                this.csvFramesDictionary.Add(symbol, frame);
            }
        }
    }
}
