using System.Text.Json.Serialization;

using Shimakaze.Milky.Model.Common;

namespace Shimakaze.Milky.Model.Message;

/// <summary>
/// 群消息
/// </summary>
/// <param name="PeerId">好友 QQ 号或群号</param>
/// <param name="MessageSeq">消息序列号</param>
/// <param name="SenderId">发送者 QQ 号</param>
/// <param name="Time">消息时间</param>
/// <param name="Segments">消息段列表</param>
/// <param name="Group">群信息</param>
/// <param name="GroupMember">群成员信息</param>
public sealed record class GroupMessage(
    long PeerId,
    long MessageSeq,
    long SenderId,
    DateTimeOffset Time,
    IncomingSegment[] Segments,
    [property: JsonPropertyName("group")] GroupEntity Group,
    [property: JsonPropertyName("group_member")] GroupMemberEntity GroupMember
) : IncomingMessage(PeerId, MessageSeq, SenderId, Time, Segments);
