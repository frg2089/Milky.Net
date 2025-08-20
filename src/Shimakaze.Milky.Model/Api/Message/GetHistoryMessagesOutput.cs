using System.Text.Json.Serialization;

using Shimakaze.Milky.Model.Message;

namespace Shimakaze.Milky.Model.Api.Message;

public sealed record class GetHistoryMessagesOutput(
    [property: JsonPropertyName("messages")] IncomingMessage[] Messages,
    [property: JsonPropertyName("next_message_seq")] long? NextMessageSeq
);
