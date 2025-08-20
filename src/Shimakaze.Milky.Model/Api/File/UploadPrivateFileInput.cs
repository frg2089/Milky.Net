using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.File;

public sealed record class UploadPrivateFileInput(
    [property: JsonPropertyName("user_id")] long UserId,
    [property: JsonPropertyName("file_uri")] string FileUri,
    [property: JsonPropertyName("file_name")] string FileName
);
