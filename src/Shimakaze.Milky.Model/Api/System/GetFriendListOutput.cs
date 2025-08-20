using System.Text.Json.Serialization;

using Shimakaze.Milky.Model.Common;

namespace Shimakaze.Milky.Model.Api.System;

/// <summary>
/// 好友列表
/// </summary>
/// <param name="Friends">好友列表</param>
public sealed record class GetFriendListOutput([property: JsonPropertyName("friends")] IReadOnlyList<FriendEntity> Friends);
