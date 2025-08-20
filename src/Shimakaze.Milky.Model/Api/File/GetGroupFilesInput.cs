using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.File;

public sealed record class GetGroupFilesInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("parent_folder_id")] string ParentFolderId = "/"
);
