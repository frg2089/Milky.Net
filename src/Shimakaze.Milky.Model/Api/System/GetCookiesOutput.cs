using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.System;

/// <summary>
/// Cookies
/// </summary>
/// <param name="Cookies">域名对应的 Cookies 字符串</param>
public sealed record class GetCookiesOutput([property: JsonPropertyName("cookies")] string Cookies);
