using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

/// <summary>
/// 接收消息基类
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "message_scene")]
[JsonDerivedType(typeof(FriendMessage), "friend")]
[JsonDerivedType(typeof(GroupMessage), "group")]
[JsonDerivedType(typeof(TempMessage), "temp")]
public abstract record class IncomingMessage(
    [property: JsonPropertyName("peer_id")] long PeerId,
    [property: JsonPropertyName("message_seq")] long MessageSeq,
    [property: JsonPropertyName("sender_id")] long SenderId,
    [property: JsonPropertyName("time")][property: JsonConverter(typeof(SecondTimestampDateTimeOffsetJsonConverter))] DateTimeOffset Time,
    [property: JsonPropertyName("segments")] IncomingSegment[] Segments);
