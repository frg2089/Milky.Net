namespace Milky.Net.Server;

public interface IFileFetcher
{
    Task<Stream> FetchFileAsync(Uri fileUri, CancellationToken cancellationToken = default);
}