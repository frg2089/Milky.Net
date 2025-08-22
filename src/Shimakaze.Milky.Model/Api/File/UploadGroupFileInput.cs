using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.File;

public sealed record class UploadGroupFileInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("file_uri")] Uri FileUri,
    [property: JsonPropertyName("file_name")] string FileName,
    [property: JsonPropertyName("parent_folder_id")] string ParentFolderId = "/"
);
