using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Event;

/// <summary>
/// 群戳一戳事件
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="SenderId">发送者 QQ 号</param>
/// <param name="ReceiverId">接收者 QQ 号</param>
public sealed record class GroupNudgeEvent(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("sender_id")] long SenderId,
    [property: JsonPropertyName("receiver_id")] long ReceiverId);
