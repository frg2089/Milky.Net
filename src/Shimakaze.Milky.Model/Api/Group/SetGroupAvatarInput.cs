using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Group;

public sealed record class SetGroupAvatarInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("image_uri")] string ImageUri
);
