using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Common;

/// <summary>
/// 群文件夹实体
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="FolderId">文件夹 ID</param>
/// <param name="ParentFolderId">父文件夹 ID</param>
/// <param name="FolderName">文件夹名称</param>
/// <param name="CreatedTime">创建时的 Unix 时间戳（秒）</param>
/// <param name="LastModifiedTime">最后修改时的 Unix 时间戳（秒）</param>
/// <param name="CreatorId">创建者 QQ 号</param>
/// <param name="FileCount">文件数量</param>
public sealed record class GroupFolderEntity(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("folder_id")] string FolderId,
    [property: JsonPropertyName("parent_folder_id")] string ParentFolderId,
    [property: JsonPropertyName("folder_name")] string FolderName,
    [property: JsonPropertyName("created_time")] DateTimeOffset CreatedTime,
    [property: JsonPropertyName("last_modified_time")] DateTimeOffset LastModifiedTime,
    [property: JsonPropertyName("creator_id")] long CreatorId,
    [property: JsonPropertyName("file_count")] int FileCount);
