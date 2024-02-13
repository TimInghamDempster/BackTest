using System.IO;
using static BackTest.CsvParser;

namespace BackTest
{
    internal interface IDataSource
    {
        internal IReadOnlyDictionary<CompanyName, CompanyData> GetCompanies();
    }

    // Holds code that can't be unit tested because it talks
    // to the file system
    internal class FileSpecificDataSource : IDataSource
    {
        private record struct Company(string Path);

        private IReadOnlyDictionary<CompanyName, CompanyData> _companies;

        // TODO: handle errors like file not found.  Basic premise will
        // be to move this from a constructor to a factory method and
        // return a result monad
        internal FileSpecificDataSource(string path)
        {
            _companies = GetCompanies(path).Select(
                f => Parse(new(Path.GetFileNameWithoutExtension(f.Path)), ReadAllLines(f))).
                DistinctBy(c => c.Name).
                ToDictionary(c => c.Name, c => c);
        }

        IEnumerable<Row> ReadAllLines(Company company) =>
            File.ReadAllLines(company.Path).Select(l => new Row(l));

        IEnumerable<Company> GetCompanies(string path) =>
            Directory.EnumerateFiles(
                path, "*.csv", SearchOption.AllDirectories).
            Select(f => new Company(f));

        IReadOnlyDictionary<CompanyName, CompanyData> IDataSource.GetCompanies() =>
            _companies;
    }
}
