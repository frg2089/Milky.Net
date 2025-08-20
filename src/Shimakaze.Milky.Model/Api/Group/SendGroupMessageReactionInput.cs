using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Group;

public sealed record class SendGroupMessageReactionInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("message_seq")] long MessageSeq,
    [property: JsonPropertyName("reaction")] string Reaction,
    [property: JsonPropertyName("is_add")] bool IsAdd = true
);
