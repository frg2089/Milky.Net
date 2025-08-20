using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Group;

public sealed record class DeleteGroupAnnouncementInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("announcement_id")] string AnnouncementId
);
