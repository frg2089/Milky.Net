using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Friend;


/// <summary>
/// 
/// </summary>
/// <param name="UserId">好友 QQ 号</param>
/// <param name="IsSelf">是否戳自己</param>
public sealed record class SendFriendNudgeInput([property: JsonPropertyName("user_id")] long UserId, [property: JsonPropertyName("is_self")] bool IsSelf);
