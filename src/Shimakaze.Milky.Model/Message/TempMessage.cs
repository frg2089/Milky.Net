using System.Text.Json.Serialization;

using Shimakaze.Milky.Model.Common;

namespace Shimakaze.Milky.Model.Message;

/// <summary>
/// 临时会话消息
/// </summary>
/// <param name="PeerId">好友 QQ 号或群号</param>
/// <param name="MessageSeq">消息序列号</param>
/// <param name="SenderId">发送者 QQ 号</param>
/// <param name="Time">消息时间</param>
/// <param name="Segments">消息段列表</param>
/// <param name="Group">临时会话发送者的所在的群信息</param>
public sealed record class TempMessage(
    long PeerId,
    long MessageSeq,
    long SenderId,
    DateTimeOffset Time,
    IncomingSegment[] Segments,
    [property: JsonPropertyName("group")] GroupEntity? Group = null
) : IncomingMessage(PeerId, MessageSeq, SenderId, Time, Segments);
