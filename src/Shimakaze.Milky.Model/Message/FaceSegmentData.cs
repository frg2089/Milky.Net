using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

public sealed record class FaceSegmentData(
    [property: JsonPropertyName("face_id")] string FaceId
);
