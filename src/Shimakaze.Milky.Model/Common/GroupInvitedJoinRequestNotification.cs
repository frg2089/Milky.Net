using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Common;

/// <summary>
/// 群成员邀请他人入群请求
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="NotificationSeq">通知序列号</param>
/// <param name="InitiatorId">邀请者 QQ 号</param>
/// <param name="TargetUserId">被邀请用户 QQ 号</param>
/// <param name="State">请求状态</param>
/// <param name="OperatorId">处理请求的管理员 QQ 号</param>
public sealed record class GroupInvitedJoinRequestNotification(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("notification_seq")] long NotificationSeq,
    [property: JsonPropertyName("initiator_id")] long InitiatorId,
    [property: JsonPropertyName("target_user_id")] long TargetUserId,
    [property: JsonPropertyName("state")] FriendRequestState State,
    [property: JsonPropertyName("operator_id")] long? OperatorId
) : GroupNotification;
