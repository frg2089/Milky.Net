using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

public sealed record class IncomingReplySegmentData(
    [property: JsonPropertyName("message_seq")] long MessageSeq
);
