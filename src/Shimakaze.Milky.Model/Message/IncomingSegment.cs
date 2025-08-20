using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

/// <summary>
/// 接收消息段基类
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(IncomingSegment<IncomingTextSegmentData>), "text")]
[JsonDerivedType(typeof(IncomingSegment<IncomingMentionSegmentData>), "mention")]
[JsonDerivedType(typeof(IncomingSegment<object>), "mention_all")]
[JsonDerivedType(typeof(IncomingSegment<IncomingFaceSegmentData>), "face")]
[JsonDerivedType(typeof(IncomingSegment<IncomingReplySegmentData>), "reply")]
[JsonDerivedType(typeof(IncomingSegment<IncomingImageSegmentData>), "image")]
[JsonDerivedType(typeof(IncomingSegment<IncomingRecordSegmentData>), "record")]
[JsonDerivedType(typeof(IncomingSegment<IncomingVideoSegmentData>), "video")]
[JsonDerivedType(typeof(IncomingSegment<IncomingForwardSegmentData>), "forward")]
[JsonDerivedType(typeof(IncomingSegment<IncomingMarketFaceSegmentData>), "market_face")]
[JsonDerivedType(typeof(IncomingSegment<IncomingLightAppSegmentData>), "light_app")]
[JsonDerivedType(typeof(IncomingSegment<IncomingXmlSegmentData>), "xml")]
public abstract record class IncomingSegment;
public sealed record class IncomingSegment<T>(
    [property: JsonPropertyName("data")] T Data
) : IncomingSegment;
