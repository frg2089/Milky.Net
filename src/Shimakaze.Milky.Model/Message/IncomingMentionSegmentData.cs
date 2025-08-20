using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

public sealed record class IncomingMentionSegmentData(
    [property: JsonPropertyName("user_id")] long UserId
);
