namespace BackTesting.Model.Utils
{
    using System.IO;
    using System.Text;
    using Deedle;

    public static class Csv2Frame
    {
        public static Frame<int, string> LoadFromFile(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                return LoadFromStream(fs);
            }
        }

        public static Frame<int, string> LoadFromString(string csvString)
        {
            using (var ms = new MemoryStream(Encoding.Default.GetBytes(csvString)))
            {
                return LoadFromStream(ms);
            }
        }

        public static Frame<int, string> LoadFromStream(Stream stream)
        {
            return Frame.ReadCsv(stream);
        }
    }
}
