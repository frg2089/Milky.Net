using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.File;

public sealed record class CreateGroupFolderOutput(
    [property: JsonPropertyName("folder_id")] string FolderId
);
