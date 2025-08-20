using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.File;

public sealed record class RenameGroupFolderInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("folder_id")] string FolderId,
    [property: JsonPropertyName("new_folder_name")] string NewFolderName
);
