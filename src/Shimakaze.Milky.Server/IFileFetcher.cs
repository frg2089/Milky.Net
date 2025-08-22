namespace Shimakaze.Milky.Server;

public interface IFileFetcher
{
    Task<Stream> FetchFileAsync(Uri fileUri, CancellationToken cancellationToken = default);
}