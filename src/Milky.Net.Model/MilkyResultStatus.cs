using System.Text.Json.Serialization;

namespace Milky.Net.Model;

[JsonConverter(typeof(JsonStringEnumConverter<MilkyResultStatus>))]
public enum MilkyResultStatus
{
    [JsonStringEnumMemberName("failed")]
    Failed = -1,
    [JsonStringEnumMemberName("ok")]
    Ok,
}
