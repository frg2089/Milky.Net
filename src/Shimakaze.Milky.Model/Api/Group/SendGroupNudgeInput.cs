using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Group;

public sealed record class SendGroupNudgeInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("user_id")] long UserId
);
