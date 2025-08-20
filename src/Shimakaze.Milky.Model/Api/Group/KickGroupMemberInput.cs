using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Group;

public sealed record class KickGroupMemberInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("user_id")] long UserId,
    [property: JsonPropertyName("reject_add_request")] bool RejectAddRequest = false
);
