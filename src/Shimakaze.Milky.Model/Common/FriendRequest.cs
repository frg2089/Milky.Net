using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Common;

/// <summary>
/// 好友请求实体
/// </summary>
/// <param name="Time">请求发起时间</param>
/// <param name="InitiatorId">请求发起者 QQ 号</param>
/// <param name="InitiatorUid">请求发起者 UID</param>
/// <param name="TargetUserId">目标用户 QQ 号</param>
/// <param name="TargetUserUid">目标用户 UID</param>
/// <param name="State">请求状态</param>
/// <param name="Comment">申请附加信息</param>
/// <param name="Via">申请来源</param>
/// <param name="IsFiltered">请求是否被过滤（发起自风险账户）</param>
public sealed record class FriendRequest(
    [property: JsonPropertyName("time")][property: JsonConverter(typeof(SecondTimestampDateTimeOffsetJsonConverter))] DateTimeOffset Time,
    [property: JsonPropertyName("initiator_id")] long InitiatorId,
    [property: JsonPropertyName("initiator_uid")] string InitiatorUid,
    [property: JsonPropertyName("target_user_id")] long TargetUserId,
    [property: JsonPropertyName("target_user_uid")] string TargetUserUid,
    [property: JsonPropertyName("state")] FriendRequestState State,
    [property: JsonPropertyName("comment")] string Comment,
    [property: JsonPropertyName("via")] string Via,
    [property: JsonPropertyName("is_filtered")] bool IsFiltered);
