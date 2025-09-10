using Milky.Net.Model;

namespace Milky.Net.Server;

public interface IFileFetcher
{
    Task<Stream> FetchFileAsync(MilkyUri uri, CancellationToken cancellationToken = default);
}