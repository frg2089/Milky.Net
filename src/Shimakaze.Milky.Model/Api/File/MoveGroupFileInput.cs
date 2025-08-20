using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.File;

public sealed record class MoveGroupFileInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("file_id")] string FileId,
    [property: JsonPropertyName("parent_folder_id")] string ParentFolderId = "/",
    [property: JsonPropertyName("target_folder_id")] string TargetFolderId = "/"
);
