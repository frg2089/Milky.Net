using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Event;

/// <summary>
/// 群名称变更事件
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="NewGroupName">新的群名称</param>
/// <param name="OperatorId">操作者 QQ 号</param>
public sealed record class GroupNameChangeEvent(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("new_group_name")] string NewGroupName,
    [property: JsonPropertyName("operator_id")] long OperatorId);
