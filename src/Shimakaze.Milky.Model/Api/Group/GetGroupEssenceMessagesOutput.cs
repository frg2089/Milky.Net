using System.Text.Json.Serialization;

using Shimakaze.Milky.Model.Message;

namespace Shimakaze.Milky.Model.Api.Group;

public sealed record class GetGroupEssenceMessagesOutput(
    [property: JsonPropertyName("messages")] GroupEssenceMessage[] Messages,
    [property: JsonPropertyName("is_end")] bool IsEnd
);
