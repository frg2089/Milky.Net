using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Common;

/// <summary>
/// 群成员被移除通知
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="NotificationSeq">通知序列号</param>
/// <param name="TargetUserId">被移除用户 QQ 号</param>
/// <param name="OperatorId">移除用户的管理员 QQ 号</param>
public sealed record class GroupKickNotification(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("notification_seq")] long NotificationSeq,
    [property: JsonPropertyName("target_user_id")] long TargetUserId,
    [property: JsonPropertyName("operator_id")] long OperatorId
) : GroupNotification;
