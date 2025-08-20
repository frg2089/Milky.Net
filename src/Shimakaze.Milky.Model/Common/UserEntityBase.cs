using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Common;

/// <summary>
/// 基础用户实体
/// </summary>
/// <param name="UserId">用户 QQ 号</param>
/// <param name="NickName">用户昵称</param>
/// <param name="Sex">用户性别</param>
public abstract record class UserEntityBase(
    [property: JsonPropertyName("user_id")] long UserId,
    [property: JsonPropertyName("nickname")] string NickName,
    [property: JsonPropertyName("sex")] Sex Sex);
