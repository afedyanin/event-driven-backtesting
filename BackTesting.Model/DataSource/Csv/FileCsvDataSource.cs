namespace BackTesting.Model.DataSource.Csv
{
    using System.IO;
    using BackTesting.Model.Utils;

    public class FileCsvDataSource : CsvDataSource
    {
        private readonly string csvDirectory;
        private readonly string[] symbolList;

        public FileCsvDataSource(string csvDirectory, string[] symbolList)
        {
            this.csvDirectory = csvDirectory;
            this.symbolList = symbolList;

            this.OpenConvertCsvFiles();
        }

        private void OpenConvertCsvFiles()
        {
            foreach (var symbol in this.symbolList)
            {
                var csvPath = Path.Combine(this.csvDirectory, symbol + ".csv");
                this.Append(symbol, csvPath);
            }
        }

        private void Append(string symbol, string filePath)
        {
            var frame = Csv2Frame.LoadFromFile(filePath);
            this.AddOrReplace(symbol, frame);
        }
    }
}
