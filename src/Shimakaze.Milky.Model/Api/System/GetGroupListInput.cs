namespace Shimakaze.Milky.Model.Api.System;

/// <summary>
/// 
/// </summary>
/// <param name="NoCache">是否强制不使用缓存</param>
public sealed record class GetGroupListInput(bool NoCache = false)
    : CachedApiBase(NoCache);
