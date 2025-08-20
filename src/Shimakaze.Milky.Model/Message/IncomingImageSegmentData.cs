using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

public sealed record class IncomingImageSegmentData(
    string ResourceId,
    string TempUrl,
    [property: JsonPropertyName("width")] int Width,
    [property: JsonPropertyName("height")] int Height,
    [property: JsonPropertyName("summary")] string Summary,
    [property: JsonPropertyName("sub_type")] string SubType
) : IncomingResourceSegmentBase(ResourceId, TempUrl);
