using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Event;

/// <summary>
/// 群精华消息变更事件
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="MessageSeq">发生变更的消息序列号</param>
/// <param name="IsSet">是否被设置为精华，false 表示被取消精华</param>
public sealed record class GroupEssenceMessageChangeEvent(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("message_seq")] long MessageSeq,
    [property: JsonPropertyName("is_set")] bool IsSet);
