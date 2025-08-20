using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.System;

/// <summary>
/// 登录信息
/// </summary>
/// <param name="Uin">登录 QQ 号</param>
/// <param name="NickName">登录昵称</param>
public sealed record class GetLoginInfoOutput(
    [property: JsonPropertyName("uin")] long Uin,
    [property: JsonPropertyName("nickname")] string NickName);
