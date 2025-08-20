using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.File;

public sealed record class DeleteGroupFolderInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("folder_id")] string FolderId
);
