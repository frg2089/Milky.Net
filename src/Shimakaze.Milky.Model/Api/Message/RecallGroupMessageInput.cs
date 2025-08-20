using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Message;

public sealed record class RecallGroupMessageInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("message_seq")] long MessageSeq
);
