using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Event;

/// <summary>
/// 群成员邀请他人入群事件
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="NotificationSeq">请求对应的通知序列号</param>
/// <param name="InitiatorId">邀请者 QQ 号</param>
/// <param name="TargetUserId">被邀请者 QQ 号</param>
public sealed record class GroupInvitedJoinRequestEvent(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("notification_seq")] long NotificationSeq,
    [property: JsonPropertyName("initiator_id")] long InitiatorId,
    [property: JsonPropertyName("target_user_id")] long TargetUserId);
