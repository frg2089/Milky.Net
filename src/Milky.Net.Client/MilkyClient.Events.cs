using System.Net.ServerSentEvents;
using System.Net.WebSockets;
using System.Text.Json;

using Milky.Net.Model;

namespace Milky.Net.Client;

public sealed partial class MilkyClient
{
    /// <summary>
    /// Milky 事件
    /// </summary>
    public IMilkyEventScheduler Events { get; }

    /// <summary>
    /// 通过 Server-Sent Events 接收事件
    /// </summary>
    /// <remarks>
    /// 此异步方法不会主动结束
    /// </remarks>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    public async Task ReceivingEventUsingSSEAsync(CancellationToken cancellationToken = default)
    {
#if NET5_0_OR_GREATER
        await using var stream = await _client.GetStreamAsync("/event", cancellationToken);
#else
        using var stream = await _client.GetStreamAsync("/event");
#endif

        var parser = SseParser.Create(stream, (eventType, data) =>
        {
            if (eventType is "heart_beat")
                return null;

            var json = JsonElement.Parse(data);
            if (eventType is "milky_event")
                return json.Deserialize(JsonSerializerOptions.GetTypeInfo<Event>());

            // Undefined behavior
            return json.Deserialize(JsonSerializerOptions.GetTypeInfo<Event>());
        });

        await foreach (var item in parser.EnumerateAsync(cancellationToken))
        {
            if (item.Data is { } data)
                Events.Received(data);
        }
    }

    /// <summary>
    /// 通过 WebSocket 接收事件
    /// </summary>
    /// <remarks>
    /// 此异步方法不会主动结束
    /// </remarks>
    /// <param name="uri">WebSocket 目标地址</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    public async Task ReceivingEventUsingWebSocketAsync(CancellationToken cancellationToken = default)
    {
        if (_client.BaseAddress is null)
            throw new InvalidOperationException("请先设置 HttpClient.BaseAddress");

        using ClientWebSocket ws = new();
#if NET5_0_OR_GREATER
        Uri uri = new(_client.BaseAddress, "/event");
        await ws.ConnectAsync(uri, _client, cancellationToken);
#else
        Uri uri = _client.DefaultRequestHeaders.Authorization is null
            ? new(_client.BaseAddress, $"/event")
            : new(_client.BaseAddress, $"/event?access_token={_client.DefaultRequestHeaders.Authorization}");
        await ws.ConnectAsync(uri, cancellationToken);
#endif

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
            await ms.WriteAsync(buffer.Array.AsMemory(0, result.Count), cancellationToken);
#endif
            if (result.EndOfMessage)
            {
                await ms.FlushAsync(cancellationToken);
                ms.Seek(0, SeekOrigin.Begin);

                if (ms.Length > 0
                    && await JsonSerializer.DeserializeAsync(ms, JsonSerializerOptions.GetTypeInfo<Event>(), cancellationToken) is { } data)
                    Events.Received(data);

                ms.SetLength(0);
            }
        }
    }
}
