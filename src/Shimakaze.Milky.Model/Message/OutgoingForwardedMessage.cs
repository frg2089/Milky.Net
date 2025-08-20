using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

/// <summary>
/// 发送转发消息
/// </summary>
/// <param name="UserId">发送者 QQ 号</param>
/// <param name="SenderName">发送者名称</param>
/// <param name="Segments">消息段列表</param>
public sealed record class OutgoingForwardedMessage(
    [property: JsonPropertyName("user_id")] long UserId,
    [property: JsonPropertyName("sender_name")] string SenderName,
    [property: JsonPropertyName("segments")] OutgoingSegment[] Segments);

