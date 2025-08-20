using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Group;

public sealed record class SetGroupNameInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("new_group_name")] string NewGroupName
);
