using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Event;

/// <summary>
/// 好友请求事件
/// </summary>
/// <param name="InitiatorId">申请好友的用户 QQ 号</param>
/// <param name="InitiatorUid">用户 UID</param>
/// <param name="Comment">申请附加信息</param>
/// <param name="Via">申请来源</param>
public sealed record class FriendRequestEvent(
    [property: JsonPropertyName("initiator_id")] long InitiatorId,
    [property: JsonPropertyName("initiator_uid")] string InitiatorUid,
    [property: JsonPropertyName("comment")] string Comment,
    [property: JsonPropertyName("via")] string Via);
