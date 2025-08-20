using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Common;

/// <summary>
/// 好友分组实体
/// </summary>
/// <param name="Id">好友分组 ID</param>
/// <param name="Name">好友分组名称</param>
public sealed record class FriendCategoryEntity(
    [property: JsonPropertyName("category_id")] int Id,
    [property: JsonPropertyName("category_name")] string Name);
