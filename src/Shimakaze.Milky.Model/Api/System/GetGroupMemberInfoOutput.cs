using System.Text.Json.Serialization;

using Shimakaze.Milky.Model.Common;

namespace Shimakaze.Milky.Model.Api.System;

/// <summary>
/// 群成员信息
/// </summary>
/// <param name="Member">群成员信息</param>
public sealed record class GetGroupMemberInfoOutput([property: JsonPropertyName("member")] GroupMemberEntity Member);
