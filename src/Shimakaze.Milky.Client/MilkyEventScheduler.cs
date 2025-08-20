using Shimakaze.Milky.Model.Event;
using Shimakaze.Milky.Model.Message;

namespace Shimakaze.Milky.Client;

public delegate void ReceivedEventHandler<T>(Event<T> @event);

public sealed class MilkyEventScheduler
{
    public event ReceivedEventHandler<BotOfflineEvent>? BotOffline;
    public event ReceivedEventHandler<IncomingMessage>? IncomingMessage;
    public event ReceivedEventHandler<MessageRecallEvent>? MessageRecall;
    public event ReceivedEventHandler<FriendRequestEvent>? FriendRequest;
    public event ReceivedEventHandler<GroupJoinRequestEvent>? GroupJoinRequest;
    public event ReceivedEventHandler<GroupInvitedJoinRequestEvent>? GroupInvitedJoinRequest;
    public event ReceivedEventHandler<GroupInvitationEvent>? GroupInvitation;
    public event ReceivedEventHandler<FriendNudgeEvent>? FriendNudge;
    public event ReceivedEventHandler<FriendFileUploadEvent>? FriendFileUpload;
    public event ReceivedEventHandler<GroupAdminChangeEvent>? GroupAdminChange;
    public event ReceivedEventHandler<GroupEssenceMessageChangeEvent>? GroupEssenceMessageChange;
    public event ReceivedEventHandler<GroupMemberIncreaseEvent>? GroupMemberIncrease;
    public event ReceivedEventHandler<GroupMemberDecreaseEvent>? GroupMemberDecrease;
    public event ReceivedEventHandler<GroupNameChangeEvent>? GroupNameChange;
    public event ReceivedEventHandler<GroupMessageReactionEvent>? GroupMessageReaction;
    public event ReceivedEventHandler<GroupMuteEvent>? GroupMute;
    public event ReceivedEventHandler<GroupWholeMuteEvent>? GroupWholeMute;
    public event ReceivedEventHandler<GroupNudgeEvent>? GroupNudge;
    public event ReceivedEventHandler<GroupFileUploadEvent>? GroupFileUpload;

    public void Received(Event @event)
    {
        switch (@event)
        {
            case Event<BotOfflineEvent> typedEvent:
                BotOffline?.Invoke(typedEvent);
                break;
            case Event<IncomingMessage> typedEvent:
                IncomingMessage?.Invoke(typedEvent);
                break;
            case Event<MessageRecallEvent> typedEvent:
                MessageRecall?.Invoke(typedEvent);
                break;
            case Event<FriendRequestEvent> typedEvent:
                FriendRequest?.Invoke(typedEvent);
                break;
            case Event<GroupJoinRequestEvent> typedEvent:
                GroupJoinRequest?.Invoke(typedEvent);
                break;
            case Event<GroupInvitedJoinRequestEvent> typedEvent:
                GroupInvitedJoinRequest?.Invoke(typedEvent);
                break;
            case Event<GroupInvitationEvent> typedEvent:
                GroupInvitation?.Invoke(typedEvent);
                break;
            case Event<FriendNudgeEvent> typedEvent:
                FriendNudge?.Invoke(typedEvent);
                break;
            case Event<FriendFileUploadEvent> typedEvent:
                FriendFileUpload?.Invoke(typedEvent);
                break;
            case Event<GroupAdminChangeEvent> typedEvent:
                GroupAdminChange?.Invoke(typedEvent);
                break;
            case Event<GroupEssenceMessageChangeEvent> typedEvent:
                GroupEssenceMessageChange?.Invoke(typedEvent);
                break;
            case Event<GroupMemberIncreaseEvent> typedEvent:
                GroupMemberIncrease?.Invoke(typedEvent);
                break;
            case Event<GroupMemberDecreaseEvent> typedEvent:
                GroupMemberDecrease?.Invoke(typedEvent);
                break;
            case Event<GroupNameChangeEvent> typedEvent:
                GroupNameChange?.Invoke(typedEvent);
                break;
            case Event<GroupMessageReactionEvent> typedEvent:
                GroupMessageReaction?.Invoke(typedEvent);
                break;
            case Event<GroupMuteEvent> typedEvent:
                GroupMute?.Invoke(typedEvent);
                break;
            case Event<GroupWholeMuteEvent> typedEvent:
                GroupWholeMute?.Invoke(typedEvent);
                break;
            case Event<GroupNudgeEvent> typedEvent:
                GroupNudge?.Invoke(typedEvent);
                break;
            case Event<GroupFileUploadEvent> typedEvent:
                GroupFileUpload?.Invoke(typedEvent);
                break;
        }
    }
}
