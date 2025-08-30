using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.System;

/// <summary>
/// 用户个人信息
/// </summary>
/// <param name="Nickname">昵称</param>
/// <param name="QID">QID</param>
/// <param name="Age">年龄</param>
/// <param name="Sex">性别</param>
/// <param name="Remark">备注</param>
/// <param name="Bio">个性签名</param>
/// <param name="Level">QQ 等级</param>
/// <param name="Country">国家或地区</param>
/// <param name="City">城市</param>
/// <param name="School">学校</param>
public sealed record class GetUserProfileOutput(
    [property: JsonPropertyName("nickname")] string Nickname,
    [property: JsonPropertyName("qid")] string? QID,
    [property: JsonPropertyName("age")] int Age,
    [property: JsonPropertyName("sex")] Sex Sex,
    [property: JsonPropertyName("remark")] string? Remark,
    [property: JsonPropertyName("bio")] string? Bio,
    [property: JsonPropertyName("level")] int Level,
    [property: JsonPropertyName("country")] string Country,
    [property: JsonPropertyName("city")] string City,
    [property: JsonPropertyName("school")] string School);
