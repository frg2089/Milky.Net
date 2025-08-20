using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

public sealed record class IncomingRecordSegmentData(
    string ResourceId,
    string TempUrl,
    [property: JsonPropertyName("duration")] int Duration
) : IncomingResourceSegmentBase(ResourceId, TempUrl);
