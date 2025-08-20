using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

public sealed record class MentionSegmentData(
    [property: JsonPropertyName("user_id")] long UserId
);
