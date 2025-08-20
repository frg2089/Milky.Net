using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

/// <summary>
/// 接收资源消息段基础
/// </summary>
/// <param name="ResourceId">资源 ID</param>
/// <param name="TempUrl">临时 URL</param>
public abstract record class IncomingResourceSegmentBase(
    [property: JsonPropertyName("resource_id")] string ResourceId,
    [property: JsonPropertyName("temp_url")] string TempUrl);

