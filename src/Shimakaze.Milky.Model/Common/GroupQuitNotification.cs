using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Common;

/// <summary>
/// 群成员退群通知
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="NotificationSeq">通知序列号</param>
/// <param name="TargetUserId">退群用户 QQ 号</param>
public sealed record class GroupQuitNotification(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("notification_seq")] long NotificationSeq,
    [property: JsonPropertyName("target_user_id")] long TargetUserId
) : GroupNotification;
