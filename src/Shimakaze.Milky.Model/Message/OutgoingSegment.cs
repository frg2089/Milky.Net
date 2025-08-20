using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

/// <summary>
/// 发送消息段基类
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(OutgoingSegment<TextSegmentData>), "text")]
[JsonDerivedType(typeof(OutgoingSegment<MentionSegmentData>), "mention")]
[JsonDerivedType(typeof(OutgoingSegment<object>), "mention_all")]
[JsonDerivedType(typeof(OutgoingSegment<FaceSegmentData>), "face")]
[JsonDerivedType(typeof(OutgoingSegment<ReplySegmentData>), "reply")]
[JsonDerivedType(typeof(OutgoingSegment<ImageSegmentData>), "image")]
[JsonDerivedType(typeof(OutgoingSegment<OutgoingResourceSegmentBase>), "record")]
[JsonDerivedType(typeof(OutgoingSegment<VideoSegmentData>), "video")]
[JsonDerivedType(typeof(OutgoingSegment<ForwardSegmentData>), "forward")]
public abstract record class OutgoingSegment;
public sealed record class OutgoingSegment<T>(
    [property: JsonPropertyName("data")] T Data
) : OutgoingSegment;