using System.Text.Json.Serialization;

using Shimakaze.Milky.Model.Common;

namespace Shimakaze.Milky.Model.Api.System;

/// <summary>
/// 群成员列表
/// </summary>
/// <param name="Members">群成员列表</param>
public sealed record class GetGroupMemberListOutput([property: JsonPropertyName("members")] IReadOnlyList<GroupMemberEntity> Members);
