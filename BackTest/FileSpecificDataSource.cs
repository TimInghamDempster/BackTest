using System.IO;

namespace BackTest
{
    internal interface IDataSource
    {
        internal IEnumerable<CompanyData> GetCompanies();
    }

    // Holds code that can't be unit tested because it talks
    // to the file system
    internal class FileSpecificDataSource : IDataSource
    {
        private IEnumerable<CompanyData> _companies;

        // TODO: handle errors like file not found.  Basic premise will
        // be to move this from a constructor to a factory method and
        // return a result monad
        internal FileSpecificDataSource(string path)
        {
            _companies = GetCompanies(path).Select(
                f => CsvParser.Parse(ReadAllLines(f)));
        }

        IEnumerable<string> ReadAllLines(string path) =>
            File.ReadAllLines(path);

        IEnumerable<string> GetCompanies(string path) =>
            Directory.EnumerateFiles(
                path, "*.csv", SearchOption.AllDirectories);

        IEnumerable<CompanyData> IDataSource.GetCompanies() =>
            _companies;
    }
}
