using System.Text.Json.Serialization;

using Shimakaze.Milky.Model.Common;

namespace Shimakaze.Milky.Model.Api.Friend;
/// <summary>
/// 好友请求列表
/// </summary>
/// <param name="Requests">好友请求列表</param>
public sealed record class GetFriendRequestsOutput([property: JsonPropertyName("requests")] IReadOnlyList<FriendRequest> Requests);