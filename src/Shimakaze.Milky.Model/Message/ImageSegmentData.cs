using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

public sealed record class ImageSegmentData(
    Uri Uri,
    [property: JsonPropertyName("summary")] string? Summary,
    [property: JsonPropertyName("sub_type")] string SubType
) : OutgoingResourceSegmentBase(Uri);
