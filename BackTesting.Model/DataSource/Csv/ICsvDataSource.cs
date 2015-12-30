namespace BackTesting.Model.DataSource.Csv
{
    using System.Collections.Generic;
    using Deedle;

    public interface ICsvDataSource
    {
        IDictionary<string, Frame<int, string>> CsvFrames { get; } 
    }
}
