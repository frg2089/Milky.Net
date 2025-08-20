using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Common;

/// <summary>
/// 群实体
/// </summary>
/// <param name="Id">群号</param>
/// <param name="Name">群名称</param>
/// <param name="Count">群成员数量</param>
/// <param name="Capacity">群容量</param>
public sealed record class GroupEntity(
    [property: JsonPropertyName("group_id")] long Id,
    [property: JsonPropertyName("group_name")] string Name,
    [property: JsonPropertyName("member_count")] int Count,
    [property: JsonPropertyName("max_member_count")] int Capacity);
