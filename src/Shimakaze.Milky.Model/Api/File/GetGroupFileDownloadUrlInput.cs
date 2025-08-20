using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.File;

public sealed record class GetGroupFileDownloadUrlInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("file_id")] string FileId
);
