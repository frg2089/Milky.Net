using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

public sealed record class IncomingFaceSegmentData(
    [property: JsonPropertyName("face_id")] string FaceId
);
