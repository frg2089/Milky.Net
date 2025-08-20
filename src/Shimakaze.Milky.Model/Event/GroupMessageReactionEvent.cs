using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Event;

/// <summary>
/// 群消息回应事件
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="UserId">发送回应者 QQ 号</param>
/// <param name="MessageSeq">消息序列号</param>
/// <param name="FaceId">表情 ID</param>
/// <param name="IsAdd">是否为添加，false 表示取消回应</param>
public sealed record class GroupMessageReactionEvent(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("user_id")] long UserId,
    [property: JsonPropertyName("message_seq")] long MessageSeq,
    [property: JsonPropertyName("face_id")] string FaceId,
    [property: JsonPropertyName("is_add")] bool IsAdd);
