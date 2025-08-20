using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model;

/// <summary>
/// 群成员权限等级
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<Role>))]
public enum Role
{
    [JsonStringEnumMemberName("member")]
    Member,

    [JsonStringEnumMemberName("admin")]
    Administrator,

    [JsonStringEnumMemberName("owner")]
    Owner
}
