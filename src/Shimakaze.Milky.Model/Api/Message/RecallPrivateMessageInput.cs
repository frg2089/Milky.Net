using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Message;

public sealed record class RecallPrivateMessageInput(
    [property: JsonPropertyName("user_id")] long UserId,
    [property: JsonPropertyName("message_seq")] long MessageSeq
);
