using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Event;

/// <summary>
/// 群成员增加事件
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="UserId">发生变更的用户 QQ 号</param>
/// <param name="OperatorId">管理员 QQ 号，如果是管理员同意入群</param>
/// <param name="InvitorId">邀请者 QQ 号，如果是邀请入群</param>
public sealed record class GroupMemberIncreaseEvent(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("user_id")] long UserId,
    [property: JsonPropertyName("operator_id")] long? OperatorId,
    [property: JsonPropertyName("invitor_id")] long? InvitorId);
