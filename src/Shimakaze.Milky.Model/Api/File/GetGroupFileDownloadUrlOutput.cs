using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.File;

public sealed record class GetGroupFileDownloadUrlOutput(
    [property: JsonPropertyName("download_url")] Uri DownloadUrl
);
