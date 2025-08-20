using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Event;

/// <summary>
/// 群禁言事件
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="UserId">发生变更的用户 QQ 号</param>
/// <param name="OperatorId">操作者 QQ 号</param>
/// <param name="Duration">禁言时长（秒），为 0 表示取消禁言</param>
public sealed record class GroupMuteEvent(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("user_id")] long UserId,
    [property: JsonPropertyName("operator_id")] long OperatorId,
    [property: JsonPropertyName("duration")] int Duration);
