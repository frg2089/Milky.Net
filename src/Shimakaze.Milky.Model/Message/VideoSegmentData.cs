using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

public sealed record class VideoSegmentData(
    string Uri,
    [property: JsonPropertyName("thumb_uri")] string? ThumbUri
) : OutgoingResourceSegmentBase(Uri);
