using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Net.ServerSentEvents;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using Milky.Net.Model;

namespace Milky.Net.Client;


public sealed class MilkyClient(HttpClient client, IEnumerable<IMilkyClientMiddleware> middleware)
{
    public static readonly object EmptyObject = new();

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

        using var response = await client.PostAsJsonAsync($"/api/{api}", request, reqTypeInfo, cancellationToken: cancellationToken);

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

    public async Task ReceivingEventUsingSSEAsync(MilkyEventScheduler eventScheduler, CancellationToken cancellationToken = default)
    {
#if NET5_0_OR_GREATER
        await using var stream = await client.GetStreamAsync("/event", cancellationToken);
#else
        using var stream = await client.GetStreamAsync("/event");
#endif

        var parser = SseParser.Create(stream, (eventType, data) =>
        {
            if (eventType is "milky_event")
                return JsonSerializer.Deserialize(data, MilkyJsonSerializerContext.Default.Event);

            return JsonSerializer.Deserialize(data, MilkyJsonSerializerContext.Default.Event);
        });

        await foreach (var item in parser.EnumerateAsync(cancellationToken))
        {
            if (item.Data is { } data)
                eventScheduler.Received(data);
        }
    }

    public static async Task ReceivingEventUsingWebSocketAsync(Uri uri, MilkyEventScheduler eventScheduler, CancellationToken cancellationToken = default)
    {
        using ClientWebSocket ws = new();
        await ws.ConnectAsync(uri, cancellationToken);

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        await using MemoryStream ms = new();
        Memory<byte> buffer = new byte[4096];
#else
        using MemoryStream ms = new();
        ArraySegment<byte> buffer = new(new byte[4096]);
#endif
        while (!cancellationToken.IsCancellationRequested)
        {
            var result = await ws.ReceiveAsync(buffer, cancellationToken);
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            await ms.WriteAsync(buffer[..result.Count], cancellationToken);
#else
            await ms.WriteAsync(buffer.Array.Take(result.Count).ToArray(), cancellationToken);
#endif
            if (result.EndOfMessage)
            {
                await ms.FlushAsync(cancellationToken);
                ms.Seek(0, SeekOrigin.Begin);
                if (await JsonSerializer.DeserializeAsync(ms, MilkyJsonSerializerContext.Default.Event, cancellationToken) is { } data)
                    eventScheduler.Received(data);

                ms.SetLength(0);
            }
        }
    }
}
