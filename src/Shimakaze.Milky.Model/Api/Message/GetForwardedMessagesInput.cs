using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Message;

public sealed record class GetForwardedMessagesInput(
    [property: JsonPropertyName("forward_id")] string ForwardId
);
