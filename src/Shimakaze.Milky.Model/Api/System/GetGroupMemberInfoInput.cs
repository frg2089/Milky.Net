using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.System;

/// <summary>
/// 
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="UserId">群成员 QQ 号</param>
/// <param name="NoCache">是否强制不使用缓存</param>
public sealed record class GetGroupMemberInfoInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("user_id")] long UserId,
    bool NoCache)
    : CachedApiBase(NoCache);
