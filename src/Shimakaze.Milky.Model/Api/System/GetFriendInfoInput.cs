using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.System;

/// <summary>
/// 
/// </summary>
/// <param name="UserId">好友 QQ 号</param>
/// <param name="NoCache">是否强制不使用缓存</param>
public sealed record class GetFriendInfoInput(
    [property: JsonPropertyName("user_id")] long UserId,
    bool NoCache)
    : CachedApiBase(NoCache);
