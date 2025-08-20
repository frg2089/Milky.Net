using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Group;

public sealed record class SetGroupEssenceMessageInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("message_seq")] long MessageSeq,
    [property: JsonPropertyName("is_set")] bool IsSet = true
);
