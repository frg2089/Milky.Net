using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.System;

/// <summary>
/// CSRF Token
/// </summary>
/// <param name="CSRFToken">CSRF Token</param>
public sealed record class GetCSRFTokenOutput([property: JsonPropertyName("csrf_token")] string CSRFToken);
