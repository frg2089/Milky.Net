using System.Text.Json.Serialization;

using Shimakaze.Milky.Model.Message;

namespace Shimakaze.Milky.Model.Event;

/// <summary>
/// 事件
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "event_type")]
[JsonDerivedType(typeof(Event<BotOfflineEvent>), "bot_offline")]
[JsonDerivedType(typeof(Event<IncomingMessage>), "message_receive")]
[JsonDerivedType(typeof(Event<MessageRecallEvent>), "message_recall")]
[JsonDerivedType(typeof(Event<FriendRequestEvent>), "friend_request")]
[JsonDerivedType(typeof(Event<GroupJoinRequestEvent>), "group_join_request")]
[JsonDerivedType(typeof(Event<GroupInvitedJoinRequestEvent>), "group_invited_join_request")]
[JsonDerivedType(typeof(Event<GroupInvitationEvent>), "group_invitation")]
[JsonDerivedType(typeof(Event<FriendNudgeEvent>), "friend_nudge")]
[JsonDerivedType(typeof(Event<FriendFileUploadEvent>), "friend_file_upload")]
[JsonDerivedType(typeof(Event<GroupAdminChangeEvent>), "group_admin_change")]
[JsonDerivedType(typeof(Event<GroupEssenceMessageChangeEvent>), "group_essence_message_change")]
[JsonDerivedType(typeof(Event<GroupMemberIncreaseEvent>), "group_member_increase")]
[JsonDerivedType(typeof(Event<GroupMemberDecreaseEvent>), "group_member_decrease")]
[JsonDerivedType(typeof(Event<GroupNameChangeEvent>), "group_name_change")]
[JsonDerivedType(typeof(Event<GroupMessageReactionEvent>), "group_message_reaction")]
[JsonDerivedType(typeof(Event<GroupMuteEvent>), "group_mute")]
[JsonDerivedType(typeof(Event<GroupWholeMuteEvent>), "group_whole_mute")]
[JsonDerivedType(typeof(Event<GroupNudgeEvent>), "group_nudge")]
[JsonDerivedType(typeof(Event<GroupFileUploadEvent>), "group_file_upload")]
public abstract record class Event(
    [property: JsonPropertyName("time")][property: JsonConverter(typeof(SecondTimestampDateTimeOffsetJsonConverter))] DateTimeOffset Time,
    [property: JsonPropertyName("self_id")] long SelfId);

public sealed record class Event<T>(
    DateTimeOffset Time,
    long SelfId,
    [property: JsonPropertyName("data")] T Data
) : Event(Time, SelfId);