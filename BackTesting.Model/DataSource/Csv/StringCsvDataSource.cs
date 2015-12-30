namespace BackTesting.Model.DataSource.Csv
{
    using System.Collections.Generic;
    using BackTesting.Model.Utils;

    public class StringCsvDataSource : CsvDataSource
    {
        public StringCsvDataSource(IDictionary<string, string> csvData)
        {
            this.Convert2Frames(csvData);
        }

        private void Convert2Frames(IDictionary<string, string> csvData)
        {
            foreach (var kvp in csvData)
            {
                var frame = Csv2Frame.LoadFromString(kvp.Value);
                this.AddOrReplace(kvp.Key, frame);
            }
        }
    }
}
