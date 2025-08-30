using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Common;

/// <summary>
/// 群成员实体
/// </summary>
/// <param name="UserId">用户 QQ 号</param>
/// <param name="NickName">用户昵称</param>
/// <param name="Sex">用户性别</param>
/// <param name="GroupId">群号</param>
/// <param name="Card">成员备注</param>
/// <param name="Title">专属头衔</param>
/// <param name="Level">群等级，注意和 QQ 等级区分</param>
/// <param name="Role">权限等级</param>
/// <param name="JoinTime">入群时间</param>
/// <param name="LastSentTime">最后发言时间</param>
/// <param name="MuteEndTime">禁言结束时间</param>
public sealed record class GroupMemberEntity(
    long UserId,
    string NickName,
    Sex Sex,
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("card")] string Card,
    [property: JsonPropertyName("title")] string? Title,
    [property: JsonPropertyName("level")] int Level,
    [property: JsonPropertyName("role")] Role Role,
    [property: JsonPropertyName("join_time")] DateTimeOffset JoinTime,
    [property: JsonPropertyName("last_sent_time")] DateTimeOffset LastSentTime,
    [property: JsonPropertyName("shut_up_end_time")] DateTimeOffset? MuteEndTime)
    : UserEntityBase(UserId, NickName, Sex);
