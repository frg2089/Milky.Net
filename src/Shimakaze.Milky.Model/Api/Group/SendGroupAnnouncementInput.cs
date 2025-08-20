using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Group;

public sealed record class SendGroupAnnouncementInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("content")] string Content,
    [property: JsonPropertyName("image_uri")] string? ImageUri = null
);
