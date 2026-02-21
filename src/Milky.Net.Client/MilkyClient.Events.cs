using System.Net.ServerSentEvents;
using System.Net.WebSockets;
using System.Text.Json;

using Microsoft.Extensions.Logging;

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
    /// <param name="retryCount">重连次数 为 -1 时无限重连</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    public async Task ReceivingEventUsingSSEAsync(int retryCount = 3, CancellationToken cancellationToken = default)
    {
        string? latestId = null;

        using HttpRequestMessage request = new(HttpMethod.Get, "/event");

        for (int retry = 0; retry < retryCount || retryCount is -1; retry++)
        {
            if (retry is not 0)
            {
                if (string.IsNullOrWhiteSpace(latestId))
                {
                    if (_eventLogger is not null)
                        LogErrorLastEventIdIsNull(_eventLogger);
                    break;
                }

                request.Headers.Add("Last-Event-Id", latestId);
            }

            using var response = await _client.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();

#if NET5_0_OR_GREATER
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
#else
            using var stream = await response.Content.ReadAsStreamAsync();
#endif

            var parser = SseParser.Create(stream, (eventType, data) =>
            {
                if (_eventLogger is not null)
                    LogTraceReceivedEvent(_eventLogger, eventType);

                if (eventType is "heart_beat")
                    return null;

                var json = JsonElement.Parse(data);
                if (eventType is "milky_event")
                    return json.Deserialize(JsonSerializerOptions.GetTypeInfo<Event>());

                // Undefined behavior
                if (_eventLogger is not null)
                    LogWarningUndefinedBehavior(_eventLogger, eventType);
                return json.Deserialize(JsonSerializerOptions.GetTypeInfo<Event>());
            });

            try
            {
                await foreach (var item in parser.EnumerateAsync(cancellationToken))
                {
                    Interlocked.Exchange(ref latestId, item.EventId);
                    if (item.Data is { } data)
                        Events.Received(data);
                }
            }
            catch (OperationCanceledException)
            {
                // 用户取消
                if (_eventLogger is not null)
                    LogInformationUserCanceled(_eventLogger);
                break;
            }
            catch (HttpRequestException ex)
            {
                if (_eventLogger is not null)
                    LogWarningLongTimeNoData(_eventLogger, ex);
            }
            catch (Exception ex)
            {
                if (_eventLogger is not null)
                    LogErrorUnexpectedException(_eventLogger, ex);
            }
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
    public async Task ReceivingEventUsingWebSocketAsync(Action<ClientWebSocket>? configure = null, CancellationToken cancellationToken = default)
    {
        if (_client.BaseAddress is null)
            throw new InvalidOperationException("请先设置 HttpClient.BaseAddress");

        using ClientWebSocket ws = new();
        configure?.Invoke(ws);
#if NET5_0_OR_GREATER
        Uri uri = new(_client.BaseAddress, "/event");
        await ws.ConnectAsync(uri, _client, cancellationToken);
#else
        Uri uri = _client.DefaultRequestHeaders.Authorization is null
            ? new(_client.BaseAddress, $"/event")
            : new(_client.BaseAddress, $"/event?access_token={_client.DefaultRequestHeaders.Authorization}");
        await ws.ConnectAsync(uri, cancellationToken);
#endif

#if NET5_0_OR_GREATER
        await using MemoryStream ms = new();
        byte[] buffer = GC.AllocateUninitializedArray<byte>(4096);
#else
        using MemoryStream ms = new();
        byte[] buffer = new byte[4096];
#endif
        while (!cancellationToken.IsCancellationRequested)
        {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            var result = await ws.ReceiveAsync(buffer, cancellationToken);
#else
            var result = await ws.ReceiveAsync(new(buffer), cancellationToken);
#endif
            await ms.WriteAsync(buffer.AsMemory(0, result.Count), cancellationToken);

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

    /// <summary>
    /// 无法重连
    /// </summary>
    /// <param name="logger"></param>
    [LoggerMessage(LogLevel.Error, "Last event id is null, cannot reconnect")]
    private static partial void LogErrorLastEventIdIsNull(ILogger logger);

    /// <summary>
    /// 接收到一个事件
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="eventType"></param>
    [LoggerMessage(LogLevel.Trace, "Received event: {EventType}")]
    private static partial void LogTraceReceivedEvent(ILogger logger, string eventType);

    /// <summary>
    /// 服务器发送了一个未定义的事件类型
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="eventType"></param>
    [LoggerMessage(LogLevel.Warning, "Undefined behavior: {EventType}")]
    private static partial void LogWarningUndefinedBehavior(ILogger logger, string eventType);

    /// <summary>
    /// 用户取消连接
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="eventType"></param>
    [LoggerMessage(LogLevel.Information, "User canceled.")]
    private static partial void LogInformationUserCanceled(ILogger logger);

    /// <summary>
    /// 长时间未发送数据导致的连接被关闭
    /// </summary>
    /// <param name="logger"></param>
    [LoggerMessage(LogLevel.Warning, "Connection closed due to long time no data.")]
    private static partial void LogWarningLongTimeNoData(ILogger logger, Exception exception);

    /// <summary>
    /// 意料之外的异常
    /// </summary>
    /// <param name="logger"></param>
    [LoggerMessage(LogLevel.Error, "Unexpected exception.")]
    private static partial void LogErrorUnexpectedException(ILogger logger, Exception exception);
}
