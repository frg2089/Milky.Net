using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Message;

public sealed record class GetResourceTempUrlInput(
    [property: JsonPropertyName("resource_id")] string ResourceId
);
