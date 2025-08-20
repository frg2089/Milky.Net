using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using Shimakaze.Milky.Model;

namespace Shimakaze.Milky.Client;


public sealed class MilkyClient(HttpClient client, IEnumerable<IMilkyClientMiddleware> middleware)
{
    public static readonly object EmptyObject = new();

    public const string Version = "1.0.0-draft.6";

    private const string MetadataFactoryRequiresUnreferencedCode = "此方法与 AOT 不兼容，请使用带有 JsonTypeInfo<TRequest> 和 JsonTypeInfo<TResponse> 的方法。";
    private readonly ImmutableArray<IMilkyClientMiddleware> _middleware = [.. middleware];

    public MilkyClient(HttpClient client) : this(client, [])
    {
    }

    public async Task<TResponse> RequestAsync<TRequest, TResponse>(string api, TRequest request, JsonTypeInfo<TRequest> reqTypeInfo, JsonTypeInfo<TResponse> resTypeInfo, CancellationToken cancellationToken = default)
    {
        for (int i = 0; i < _middleware.Length; i++)
        {
            _middleware[i].PreRequest(api, request, reqTypeInfo, resTypeInfo);
        }

        var response = await client.PostAsJsonAsync($"/api/{api}", request, reqTypeInfo, cancellationToken: cancellationToken);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync(MilkyJsonSerializerContext.Default.JsonElement, cancellationToken: cancellationToken);

        var result = json.Deserialize(MilkyJsonSerializerContext.Default.MilkyResult)
            ?? throw new JsonException($"Cannot deserialize string as {MilkyJsonSerializerContext.Default.MilkyResult.Type.FullName}");

        var data = result.GetResult(resTypeInfo);

        for (int i = _middleware.Length - 1; i >= 0; i--)
        {
            _middleware[i].PostRequest(api, request, data, reqTypeInfo, resTypeInfo);
        }


        return data;
    }

    [RequiresUnreferencedCode(MetadataFactoryRequiresUnreferencedCode)]
    [RequiresDynamicCode(MetadataFactoryRequiresUnreferencedCode)]
    public async Task<TResponse> RequestAsync<TRequest, TResponse>(string api, TRequest request, CancellationToken cancellationToken = default)
        => await RequestAsync(api, request, JsonTypeInfo.CreateJsonTypeInfo<TRequest>(MilkyJsonSerializerContext.Default.Options), JsonTypeInfo.CreateJsonTypeInfo<TResponse>(MilkyJsonSerializerContext.Default.Options), cancellationToken);

    /// <summary>
    /// 使用 <see cref="MilkyJsonSerializerContext"/> 进行请求
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="api"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="MilkyException">找不到类型对应的 JsonTypeInfo </exception>
    public async Task<TResponse> AotCompatibleRequestAsync<TRequest, TResponse>(string api, TRequest request, CancellationToken cancellationToken = default)
    {
        if (MilkyJsonSerializerContext.Default.GetTypeInfo(typeof(TRequest)) is not JsonTypeInfo<TRequest> reqTypeInfo)
            throw new MilkyException("无法获取请求类型信息");

        if (MilkyJsonSerializerContext.Default.GetTypeInfo(typeof(TResponse)) is not JsonTypeInfo<TResponse> resTypeInfo)
            throw new MilkyException("无法获取响应类型信息");

        return await RequestAsync(api, request, reqTypeInfo, resTypeInfo, cancellationToken);
    }
}
