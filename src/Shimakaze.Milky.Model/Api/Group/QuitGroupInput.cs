using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Group;

public sealed record class QuitGroupInput(
    [property: JsonPropertyName("group_id")] long GroupId
);
