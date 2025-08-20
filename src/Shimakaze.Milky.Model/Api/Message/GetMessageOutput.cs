using System.Text.Json.Serialization;

using Shimakaze.Milky.Model.Message;

namespace Shimakaze.Milky.Model.Api.Message;

public sealed record class GetMessageOutput(
    [property: JsonPropertyName("message")] IncomingMessage Message
);
