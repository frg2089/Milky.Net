using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Event;

/// <summary>
/// 好友戳一戳事件
/// </summary>
/// <param name="UserId">好友 QQ 号</param>
/// <param name="IsSelfSend">是否是自己发送的戳一戳</param>
/// <param name="IsSelfReceive">是否是自己接收的戳一戳</param>
public sealed record class FriendNudgeEvent(
    [property: JsonPropertyName("user_id")] long UserId,
    [property: JsonPropertyName("is_self_send")] bool IsSelfSend,
    [property: JsonPropertyName("is_self_receive")] bool IsSelfReceive);
