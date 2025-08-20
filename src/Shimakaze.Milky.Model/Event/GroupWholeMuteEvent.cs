using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Event;

/// <summary>
/// 群全体禁言事件
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="OperatorId">操作者 QQ 号</param>
/// <param name="IsMute">是否全员禁言，false 表示取消全员禁言</param>
public sealed record class GroupWholeMuteEvent(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("operator_id")] long OperatorId,
    [property: JsonPropertyName("is_mute")] bool IsMute);
