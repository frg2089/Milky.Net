using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.File;

public sealed record class GetPrivateFileDownloadUrlInput(
    [property: JsonPropertyName("user_id")] long UserId,
    [property: JsonPropertyName("file_id")] string FileId,
    [property: JsonPropertyName("file_hash")] string FileHash
);
