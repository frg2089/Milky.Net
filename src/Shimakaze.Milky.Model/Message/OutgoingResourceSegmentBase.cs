using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

/// <summary>
/// 发送资源消息段基础
/// </summary>
/// <param name="Uri">文件 URI，支持 file:// http(s):// base64:// 三种格式</param>
public abstract record class OutgoingResourceSegmentBase(
    [property: JsonPropertyName("uri")] Uri Uri);
