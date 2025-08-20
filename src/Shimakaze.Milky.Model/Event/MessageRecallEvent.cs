using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Event;

/// <summary>
/// 消息撤回事件
/// </summary>
/// <param name="MessageScene">消息场景</param>
/// <param name="PeerId">好友 QQ 号或群号</param>
/// <param name="MessageSeq">消息序列号</param>
/// <param name="SenderId">被撤回的消息的发送者 QQ 号</param>
/// <param name="OperatorId">操作者 QQ 号</param>
public sealed record class MessageRecallEvent(
    [property: JsonPropertyName("message_scene")] MessageScene MessageScene,
    [property: JsonPropertyName("peer_id")] long PeerId,
    [property: JsonPropertyName("message_seq")] long MessageSeq,
    [property: JsonPropertyName("sender_id")] long SenderId,
    [property: JsonPropertyName("operator_id")] long OperatorId);
