using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Group;

public sealed record class GetGroupAnnouncementListInput(
    [property: JsonPropertyName("group_id")] long GroupId
);
