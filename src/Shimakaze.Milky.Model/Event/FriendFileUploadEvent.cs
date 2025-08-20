using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Event;

/// <summary>
/// 好友文件上传事件
/// </summary>
/// <param name="UserId">好友 QQ 号</param>
/// <param name="FileId">文件 ID</param>
/// <param name="FileName">文件名称</param>
/// <param name="FileSize">文件大小</param>
/// <param name="FileHash">文件的 TriSHA1 哈希值</param>
/// <param name="IsSelf">是否是自己发送的文件</param>
public sealed record class FriendFileUploadEvent(
    [property: JsonPropertyName("user_id")] long UserId,
    [property: JsonPropertyName("file_id")] string FileId,
    [property: JsonPropertyName("file_name")] string FileName,
    [property: JsonPropertyName("file_size")] long FileSize,
    [property: JsonPropertyName("file_hash")] string FileHash,
    [property: JsonPropertyName("is_self")] bool IsSelf);
