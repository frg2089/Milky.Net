using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.File;

public sealed record class GetPrivateFileDownloadUrlOutput(
    [property: JsonPropertyName("download_url")] Uri DownloadUrl
);
