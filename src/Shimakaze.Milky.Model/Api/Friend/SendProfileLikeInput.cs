using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Friend;

/// <summary>
/// 
/// </summary>
/// <param name="UserId">好友 QQ 号</param>
/// <param name="Count">点赞数量</param>
public sealed record class SendProfileLikeInput([property: JsonPropertyName("user_id")] long UserId, [property: JsonPropertyName("count")] int Count);
