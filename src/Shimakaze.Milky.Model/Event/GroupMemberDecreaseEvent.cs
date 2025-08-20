using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Event;

/// <summary>
/// 群成员减少事件
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="UserId">发生变更的用户 QQ 号</param>
/// <param name="OperatorId">管理员 QQ 号，如果是管理员踢出</param>
public sealed record class GroupMemberDecreaseEvent(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("user_id")] long UserId,
    [property: JsonPropertyName("operator_id")] long? OperatorId);
