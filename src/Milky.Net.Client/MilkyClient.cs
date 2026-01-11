using System.Collections.Immutable;
using System.Net.Http.Json;
using System.Text.Json;

using Milky.Net.Model;

namespace Milky.Net.Client;

/// <summary>
/// Milky 客户端
/// </summary>
public sealed partial class MilkyClient
{
    private readonly HttpClient _client;
    private readonly ImmutableArray<IMilkyClientMiddleware> _middleware;

    internal static readonly object EmptyObject = new();

    /// <summary>
    /// 向 Milky 服务器（协议端）发送请求
    /// </summary>
    /// <remarks>
    /// 此方法会自动在 <paramref name="api"/> 前加上 `/api/` 前缀
    /// </remarks>
    /// <typeparam name="TRequest">请求体类型</typeparam>
    /// <typeparam name="TResponse">响应体类型</typeparam>
    /// <param name="api">API 名</param>
    /// <param name="request">请求体对象</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    /// <exception cref="JsonException">Json 序列化失败</exception>
    public async Task<TResponse> RequestAsync<TRequest, TResponse>(string api, TRequest request, CancellationToken cancellationToken = default)
    {
        var requestTypeInfo = MilkyJsonSerializerContext.Default.GetTypeInfo(typeof(TRequest))
            ?? throw new NotSupportedException($"Type {typeof(TRequest).FullName} are not supported.");

        Func<Task<TResponse>> func = async () =>
        {
            using var body = JsonContent.Create(request, requestTypeInfo);
            using var response = await _client.PostAsync($"/api/{api}", body, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync(MilkyJsonSerializerContext.Default.JsonElement, cancellationToken: cancellationToken);

            var result = json.Deserialize(MilkyJsonSerializerContext.Default.MilkyResult)
                ?? throw new JsonException($"Cannot deserialize string as {MilkyJsonSerializerContext.Default.MilkyResult.Type.FullName}");

            var data = result.GetResult<TResponse>();

            return data;
        };

        foreach (var item in _middleware)
        {
            func = async () =>
            {
                TaskCompletionSource<TResponse> tcs = new();
                await item.Execute(api, request, async () =>
                {
                    var response = await func();
                    tcs.SetResult(response);
                    return response;
                }, cancellationToken);
                return await tcs.Task;
            };
        }

        return await func();
    }
}
