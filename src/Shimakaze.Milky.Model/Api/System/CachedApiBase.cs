using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.System;

/// <summary>
/// 
/// </summary>
/// <param name="NoCache">是否强制不使用缓存</param>
public abstract record class CachedApiBase(
    [property: JsonPropertyName("no_cache")] bool NoCache = false);
