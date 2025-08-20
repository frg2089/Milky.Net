using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Common;

/// <summary>
/// 群文件实体
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="FileId">文件 ID</param>
/// <param name="FileName">文件名称</param>
/// <param name="ParentFolderId">父文件夹 ID</param>
/// <param name="FileSize">文件大小（字节）</param>
/// <param name="UploadedTime">上传时的 Unix 时间戳（秒）</param>
/// <param name="ExpireTime">过期时的 Unix 时间戳（秒）</param>
/// <param name="UploaderId">上传者 QQ 号</param>
/// <param name="DownloadedTimes">下载次数</param>
public sealed record class GroupFileEntity(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("file_id")] string FileId,
    [property: JsonPropertyName("file_name")] string FileName,
    [property: JsonPropertyName("parent_folder_id")] string ParentFolderId,
    [property: JsonPropertyName("file_size")] long FileSize,
    [property: JsonPropertyName("uploaded_time")] DateTimeOffset UploadedTime,
    [property: JsonPropertyName("expire_time")] DateTimeOffset? ExpireTime,
    [property: JsonPropertyName("uploader_id")] long UploaderId,
    [property: JsonPropertyName("downloaded_times")] int DownloadedTimes);
