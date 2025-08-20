using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Group;

public sealed record class AcceptGroupInvitationInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("invitation_seq")] long InvitationSeq
);
