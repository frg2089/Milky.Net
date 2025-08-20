using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.System;

/// <summary>
/// 协议端信息
/// </summary>
/// <param name="ImplementName">协议端名称</param>
/// <param name="ImplementVersion">协议端版本</param>
/// <param name="QQProtocolVersion">协议端使用的 QQ 协议版本</param>
/// <param name="QQProtocolType">协议端使用的 QQ 协议平台</param>
/// <param name="MilkyVersion">协议端实现的 Milky 协议版本，目前为 "1.0"</param>
public sealed record class GetImplInfoOutput(
    [property: JsonPropertyName("impl_name")] string ImplementName,
    [property: JsonPropertyName("impl_version")] string ImplementVersion,
    [property: JsonPropertyName("qq_protocol_version")] string QQProtocolVersion,
    [property: JsonPropertyName("qq_protocol_type")] QQProtocolType QQProtocolType,
    [property: JsonPropertyName("milky_version")] string MilkyVersion);
