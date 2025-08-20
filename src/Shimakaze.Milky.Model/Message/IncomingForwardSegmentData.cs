using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

public sealed record class IncomingForwardSegmentData(
    [property: JsonPropertyName("forward_id")] string ForwardId
);
