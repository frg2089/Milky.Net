using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model;

[JsonConverter(typeof(JsonStringEnumConverter<FriendRequestState>))]
public enum FriendRequestState
{
    [JsonStringEnumMemberName("pending")]
    Pending,
    [JsonStringEnumMemberName("accepted")]
    Accepted,
    [JsonStringEnumMemberName("rejected")]
    Rejected,
    [JsonStringEnumMemberName("ignored")]
    Ignored,
}