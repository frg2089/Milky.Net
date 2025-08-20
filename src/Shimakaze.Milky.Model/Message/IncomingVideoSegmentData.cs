using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

public sealed record class IncomingVideoSegmentData(
    string ResourceId,
    string TempUrl,
    [property: JsonPropertyName("width")] int Width,
    [property: JsonPropertyName("height")] int Height,
    [property: JsonPropertyName("duration")] int Duration
) : IncomingResourceSegmentBase(ResourceId, TempUrl);
