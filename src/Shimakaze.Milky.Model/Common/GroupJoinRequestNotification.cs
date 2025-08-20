using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Common;

/// <summary>
/// 用户入群请求
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="NotificationSeq">通知序列号</param>
/// <param name="IsFiltered">请求是否被过滤（发起自风险账户）</param>
/// <param name="InitiatorId">发起者 QQ 号</param>
/// <param name="State">请求状态</param>
/// <param name="OperatorId">处理请求的管理员 QQ 号</param>
/// <param name="Comment">入群请求附加信息</param>
public sealed record class GroupJoinRequestNotification(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("notification_seq")] long NotificationSeq,
    [property: JsonPropertyName("is_filtered")] bool IsFiltered,
    [property: JsonPropertyName("initiator_id")] long InitiatorId,
    [property: JsonPropertyName("state")] FriendRequestState State,
    [property: JsonPropertyName("operator_id")] long? OperatorId,
    [property: JsonPropertyName("comment")] string Comment
) : GroupNotification;
