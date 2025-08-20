using System.Text.Json.Serialization;

using Shimakaze.Milky.Model.Message;

namespace Shimakaze.Milky.Model.Api.Message;

public abstract record class SendMessageApiBase(
    [property: JsonPropertyName("message")] OutgoingSegment[] Message
);
