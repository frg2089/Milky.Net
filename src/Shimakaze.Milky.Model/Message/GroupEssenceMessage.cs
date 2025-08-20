using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

/// <summary>
/// 群精华消息
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="MessageSeq">消息序列号</param>
/// <param name="MessageTime">消息发送时的时间</param>
/// <param name="SenderId">发送者 QQ 号</param>
/// <param name="SenderName">发送者名称</param>
/// <param name="OperatorId">设置精华的操作者 QQ 号</param>
/// <param name="OperatorName">设置精华的操作者名称</param>
/// <param name="OperationTime">消息被设置精华的时间</param>
/// <param name="Segments">消息段列表</param>
public sealed record class GroupEssenceMessage(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("message_seq")] long MessageSeq,
    [property: JsonPropertyName("message_time")] DateTimeOffset MessageTime,
    [property: JsonPropertyName("sender_id")] long SenderId,
    [property: JsonPropertyName("sender_name")] string SenderName,
    [property: JsonPropertyName("operator_id")] long OperatorId,
    [property: JsonPropertyName("operator_name")] string OperatorName,
    [property: JsonPropertyName("operation_time")] DateTimeOffset OperationTime,
    [property: JsonPropertyName("segments")] IncomingSegment[] Segments);

