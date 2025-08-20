using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

/// <summary>
/// 共享消息段基础
/// </summary>
/// <param name="Type">消息段类型</param>
public sealed record class SharedSegment(
    [property: JsonPropertyName("type")] string Type);

