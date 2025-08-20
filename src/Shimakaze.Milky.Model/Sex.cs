using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model;

/// <summary>
/// 用户性别
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<Sex>))]
public enum Sex
{
    [JsonStringEnumMemberName("male")]
    Male,

    [JsonStringEnumMemberName("female")]
    Female,

    [JsonStringEnumMemberName("unknown")]
    Unknown
}
