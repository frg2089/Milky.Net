using System.Text.Json.Serialization;

using Shimakaze.Milky.Model.Common;

namespace Shimakaze.Milky.Model.Api.System;

/// <summary>
/// 群列表
/// </summary>
/// <param name="Groups">群列表</param>
public sealed record class GetGroupListOutput([property: JsonPropertyName("groups")] IReadOnlyList<GroupEntity> Groups);
