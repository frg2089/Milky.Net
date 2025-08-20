using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.File;

public sealed record class CreateGroupFolderInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("folder_name")] string FolderName
);
