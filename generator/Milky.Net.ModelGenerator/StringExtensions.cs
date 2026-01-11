namespace Milky.Net.ModelGenerator;

internal static class StringExtensions
{
    extension(IEnumerable<string> values)
    {
        public string Join(string separator) => string.Join(separator, values);
        public string Join(char separator) => string.Join(separator, values);
    }
}