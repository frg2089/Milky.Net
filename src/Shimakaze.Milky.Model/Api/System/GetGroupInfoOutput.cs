using System.Text.Json.Serialization;

using Shimakaze.Milky.Model.Common;

namespace Shimakaze.Milky.Model.Api.System;

/// <summary>
/// 群信息
/// </summary>
/// <param name="Group">群信息</param>
public sealed record class GetGroupInfoOutput([property: JsonPropertyName("group")] GroupEntity Group);
