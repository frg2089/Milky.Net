using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Friend;

/// <summary>
/// 
/// </summary>
/// <param name="InitiatorUid">请求发起者 UID</param>
/// <param name="IsFiltered">是否是被过滤的请求</param>
public sealed record class AcceptFriendRequestInput([property: JsonPropertyName("initiator_uid")] string InitiatorUid, [property: JsonPropertyName("is_filtered")] bool IsFiltered);
