using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

/// <summary>
/// 接收转发消息
/// </summary>
/// <param name="SenderName">发送者名称</param>
/// <param name="AvatarUrl">发送者头像 URL</param>
/// <param name="Time">消息时间</param>
/// <param name="Segments">消息段列表</param>
public sealed record class IncomingForwardedMessage(
    [property: JsonPropertyName("sender_name")] string SenderName,
    [property: JsonPropertyName("avatar_url")] string AvatarUrl,
    [property: JsonPropertyName("time")] DateTimeOffset Time,
    [property: JsonPropertyName("segments")] IncomingSegment[] Segments);

