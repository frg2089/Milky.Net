using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.File;

public sealed record class RenameGroupFileInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("file_id")] string FileId,
    [property: JsonPropertyName("new_file_name")] string NewFileName,
    [property: JsonPropertyName("parent_folder_id")] string ParentFolderId = "/"
);
