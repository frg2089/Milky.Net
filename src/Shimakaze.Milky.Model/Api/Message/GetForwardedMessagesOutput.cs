using System.Text.Json.Serialization;

using Shimakaze.Milky.Model.Message;

namespace Shimakaze.Milky.Model.Api.Message;

public sealed record class GetForwardedMessagesOutput(
    [property: JsonPropertyName("messages")] IncomingForwardedMessage[] Messages
);
