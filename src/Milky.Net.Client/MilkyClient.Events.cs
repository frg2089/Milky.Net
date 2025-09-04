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
    public MilkyEventScheduler Events { get; }

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
            if (eventType is "milky_event")
                return JsonSerializer.Deserialize(data, MilkyJsonSerializerContext.Default.Event);

            return JsonSerializer.Deserialize(data, MilkyJsonSerializerContext.Default.Event);
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
    public async Task ReceivingEventUsingWebSocketAsync(Uri uri, CancellationToken cancellationToken = default)
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
                    Events.Received(data);

                ms.SetLength(0);
            }
        }
    }
}
