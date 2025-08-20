using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Message;

public sealed record class GetResourceTempUrlOutput(
    [property: JsonPropertyName("url")] string Url
);
