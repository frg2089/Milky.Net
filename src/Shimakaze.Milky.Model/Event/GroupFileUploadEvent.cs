using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Event;

/// <summary>
/// 群文件上传事件
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="UserId">发送者 QQ 号</param>
/// <param name="FileId">文件 ID</param>
/// <param name="FileName">文件名称</param>
/// <param name="FileSize">文件大小</param>
public sealed record class GroupFileUploadEvent(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("user_id")] long UserId,
    [property: JsonPropertyName("file_id")] string FileId,
    [property: JsonPropertyName("file_name")] string FileName,
    [property: JsonPropertyName("file_size")] long FileSize);
