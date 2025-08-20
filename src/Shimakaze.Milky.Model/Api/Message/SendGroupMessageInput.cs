using System.Text.Json.Serialization;

using Shimakaze.Milky.Model.Message;

namespace Shimakaze.Milky.Model.Api.Message;

public sealed record class SendGroupMessageInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    OutgoingSegment[] Message
) : SendMessageApiBase(Message);
