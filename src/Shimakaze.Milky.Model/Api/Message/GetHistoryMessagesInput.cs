using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Message;

public sealed record class GetHistoryMessagesInput(
    [property: JsonPropertyName("message_scene")] string MessageScene,
    [property: JsonPropertyName("peer_id")] long PeerId,
    [property: JsonPropertyName("start_message_seq")] long? StartMessageSeq = null,
    [property: JsonPropertyName("limit")] int Limit = 20
);
