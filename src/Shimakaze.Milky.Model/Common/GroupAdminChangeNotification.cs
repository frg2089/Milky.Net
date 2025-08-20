using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Common;

/// <summary>
/// 群管理员变更通知
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="NotificationSeq">通知序列号</param>
/// <param name="TargetUserId">被设置/取消用户 QQ 号</param>
/// <param name="IsSet">是否被设置为管理员，false 表示被取消管理员</param>
/// <param name="OperatorId">操作者（群主）QQ 号</param>
public sealed record class GroupAdminChangeNotification(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("notification_seq")] long NotificationSeq,
    [property: JsonPropertyName("target_user_id")] long TargetUserId,
    [property: JsonPropertyName("is_set")] bool IsSet,
    [property: JsonPropertyName("operator_id")] long OperatorId
) : GroupNotification;
