using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.System;

/// <summary>
/// 
/// </summary>
/// <param name="Domain">需要获取 Cookies 的域名</param>
public sealed record class GetCookiesInput(
    [property: JsonPropertyName("domain")] string Domain);
