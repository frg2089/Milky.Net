using System.Text.Json.Serialization;

using Shimakaze.Milky.Model.Message;

namespace Shimakaze.Milky.Model.Api.Message;

public sealed record class SendPrivateMessageInput(
    [property: JsonPropertyName("user_id")] long UserId,
    OutgoingSegment[] Message
) : SendMessageApiBase(Message);
