using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Common;

/// <summary>
/// 群通知实体
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(GroupJoinRequestNotification), "join_request")]
[JsonDerivedType(typeof(GroupAdminChangeNotification), "admin_change")]
[JsonDerivedType(typeof(GroupKickNotification), "kick")]
[JsonDerivedType(typeof(GroupQuitNotification), "quit")]
[JsonDerivedType(typeof(GroupInvitedJoinRequestNotification), "invited_join_request")]
public abstract record class GroupNotification;
