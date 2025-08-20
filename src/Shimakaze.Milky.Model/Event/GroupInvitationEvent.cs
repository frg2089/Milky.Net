using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Event;

/// <summary>
/// 他人邀请自身入群事件
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="InvitationSeq">邀请序列号</param>
/// <param name="InitiatorId">邀请者 QQ 号</param>
public sealed record class GroupInvitationEvent(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("invitation_seq")] long InvitationSeq,
    [property: JsonPropertyName("initiator_id")] long InitiatorId);
