using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Common;

/// <summary>
/// 好友实体
/// </summary>
/// <param name="UserId">用户 QQ 号</param>
/// <param name="NickName">用户昵称</param>
/// <param name="Sex">用户性别</param>
/// <param name="QID">用户 QID</param>
/// <param name="Remark">好友备注</param>
/// <param name="Category">好友分组</param>
public sealed record class FriendEntity(
    long UserId,
    string NickName,
    Sex Sex,
    [property: JsonPropertyName("qid")] string QID,
    [property: JsonPropertyName("remark")] string Remark,
    [property: JsonPropertyName("category")] FriendCategoryEntity Category)
    : UserEntityBase(UserId, NickName, Sex);
