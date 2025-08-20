using System.Text.Json.Serialization;

using Shimakaze.Milky.Model.Common;

namespace Shimakaze.Milky.Model.Api.System;

/// <summary>
/// 好友信息
/// </summary>
/// <param name="Friend">好友信息</param>
public sealed record class GetFriendInfoOutput([property: JsonPropertyName("friend")] FriendEntity Friend);
