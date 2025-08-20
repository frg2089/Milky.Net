using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Friend;

/// <summary>
/// 
/// </summary>
/// <param name="Limit">获取的最大请求数量</param>
/// <param name="IsFiltered">`true` 表示只获取被过滤（由风险账号发起）的通知，`false` 表示只获取未被过滤的通知</param>
public sealed record class GetFriendRequestsInput([property: JsonPropertyName("limit")] int Limit, [property: JsonPropertyName("is_filtered")] bool IsFiltered);
