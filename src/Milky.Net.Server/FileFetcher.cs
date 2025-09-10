using Milky.Net.Model;

namespace Milky.Net.Server;

public sealed class FileFetcher(HttpClient httpClient) : IFileFetcher
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    public async Task<Stream> FetchFileAsync(MilkyUri uri, CancellationToken cancellationToken = default)
    {
        return uri switch
        {
            { IsFile: true } when uri.TryGetUri(out var uri1) => await OpenFileAsync(uri1, cancellationToken),
            { IsHttp: true } or { IsHttps: true } when uri.TryGetUri(out var uri1) => await DownloadStreamAsync(uri1, cancellationToken),
            { IsBase64: true } => await GetFromBase64Async(uri, cancellationToken),
            _ => throw new NotSupportedException($"不支持的协议: {uri.Scheme}")
        };
    }

    private static Task<Stream> OpenFileAsync(Uri uri, CancellationToken cancellationToken)
    {
        // CancellationToken 对同步文件操作无效，但可提前检查
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            // 使用 Uri.LocalPath 获取解码后的本地路径
            string localPath = uri.LocalPath;

            // 验证路径是否存在
            if (!File.Exists(localPath))
                throw new FileNotFoundException($"文件未找到: {localPath}");

            // 以只读、共享方式打开，避免独占
            var fileStream = new FileStream(
                localPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 4096,
                useAsync: true);

            return Task.FromResult<Stream>(fileStream);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException($"无法读取文件 '{uri}': {ex.Message}", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new UnauthorizedAccessException($"没有权限访问文件 '{uri}': {ex.Message}", ex);
        }
    }

    private async Task<Stream> DownloadStreamAsync(Uri uri, CancellationToken cancellationToken)
    {
        try
        {
            // 使用 HttpClient 获取流（不缓冲整个内容）
            using var response = await _httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            response.EnsureSuccessStatusCode();

            // 返回响应体流，由调用方释放
#if NET8_0_OR_GREATER
            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
#else
            var stream = await response.Content.ReadAsStreamAsync();
#endif

            return stream;
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"HTTP 请求失败 '{uri}': {ex.Message}", ex);
        }
        catch (TaskCanceledException ex) when (ex.CancellationToken == cancellationToken)
        {
            // 明确是用户取消
            throw new OperationCanceledException("文件下载被取消", ex, cancellationToken);
        }
    }

    private static async Task<Stream> GetFromBase64Async(MilkyUri uri, CancellationToken cancellationToken)
    {
        var buffer = Convert.FromBase64String(uri.OriginalUri[9..]);
        MemoryStream ms = new(buffer.Length);
        await ms.WriteAsync(buffer, cancellationToken);
        ms.Seek(0, SeekOrigin.Begin);
        return ms;
    }
}