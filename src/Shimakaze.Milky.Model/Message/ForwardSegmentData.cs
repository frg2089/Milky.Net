using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

public sealed record class ForwardSegmentData(
    [property: JsonPropertyName("messages")] OutgoingForwardedMessage[] Messages
);
