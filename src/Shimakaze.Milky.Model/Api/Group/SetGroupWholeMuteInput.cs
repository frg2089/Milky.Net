using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Group;

public sealed record class SetGroupWholeMuteInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("is_mute")] bool IsMute = true
);
