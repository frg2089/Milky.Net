using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.System;

/// <summary>
/// 
/// </summary>
/// <param name="UserId">用户 QQ 号</param>
public sealed record class GetUserProfileInput(
    [property: JsonPropertyName("user_id")] long UserId);
