using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.File;

public sealed record class UploadPrivateFileOutput(
    [property: JsonPropertyName("file_id")] string FileId
);
