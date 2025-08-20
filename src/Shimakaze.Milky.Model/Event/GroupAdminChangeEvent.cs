using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Event;

/// <summary>
/// 群管理员变更事件
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="UserId">发生变更的用户 QQ 号</param>
/// <param name="IsSet">是否被设置为管理员，false 表示被取消管理员</param>
public sealed record class GroupAdminChangeEvent(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("user_id")] long UserId,
    [property: JsonPropertyName("is_set")] bool IsSet);
