using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Event;

/// <summary>
/// 入群申请事件
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="NotificationSeq">请求对应的通知序列号</param>
/// <param name="IsFiltered">请求是否被过滤（发起自风险账户）</param>
/// <param name="InitiatorId">申请入群的用户 QQ 号</param>
/// <param name="Comment">申请附加信息</param>
public sealed record class GroupJoinRequestEvent(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("notification_seq")] long NotificationSeq,
    [property: JsonPropertyName("is_filtered")] bool IsFiltered,
    [property: JsonPropertyName("initiator_id")] long InitiatorId,
    [property: JsonPropertyName("comment")] string Comment);
