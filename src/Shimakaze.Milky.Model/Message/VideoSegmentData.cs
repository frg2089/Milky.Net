using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

public sealed record class VideoSegmentData(
    Uri Uri,
    [property: JsonPropertyName("thumb_uri")] string? ThumbUri
) : OutgoingResourceSegmentBase(Uri);
