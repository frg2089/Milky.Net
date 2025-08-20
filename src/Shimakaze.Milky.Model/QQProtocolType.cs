using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model;

[JsonConverter(typeof(JsonStringEnumConverter<QQProtocolType>))]
public enum QQProtocolType
{
    [JsonStringEnumMemberName("windows")]
    Windows,
    [JsonStringEnumMemberName("linux")]
    Linux,
    [JsonStringEnumMemberName("macos")]
    MacOS,
    [JsonStringEnumMemberName("android_pad")]
    AndroidPad,
    [JsonStringEnumMemberName("android_phone")]
    AndroidPhone,
    [JsonStringEnumMemberName("ipad")]
    IPad,
    [JsonStringEnumMemberName("iphone")]
    IPhone,
    [JsonStringEnumMemberName("harmony")]
    Harmony,
    [JsonStringEnumMemberName("watch")]
    Watch,
}