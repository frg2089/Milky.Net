using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Message;

public sealed record class GetMessageInput(
    [property: JsonPropertyName("message_scene")] MessageScene MessageScene,
    [property: JsonPropertyName("peer_id")] long PeerId,
    [property: JsonPropertyName("message_seq")] long MessageSeq
);
