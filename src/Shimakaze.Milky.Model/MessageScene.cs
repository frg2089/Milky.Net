using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model;

/// <summary>
/// 消息场景枚举
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<MessageScene>))]
public enum MessageScene
{
    [JsonStringEnumMemberName("friend")]
    Friend,
    [JsonStringEnumMemberName("group")]
    Group,
    [JsonStringEnumMemberName("temp")]
    Temp
}
