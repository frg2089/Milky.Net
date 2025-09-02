using Milky.Net.Model;

namespace Milky.Net.Client;

public delegate void ReceivedEventHandler<T>(T @event) where T : Event;

public sealed class MilkyEventScheduler
{
    public event ReceivedEventHandler<BotOfflineUnionEvent>? BotOffline;
    public event ReceivedEventHandler<MessageReceiveUnionEvent>? IncomingMessage;
    public event ReceivedEventHandler<MessageRecallUnionEvent>? MessageRecall;
    public event ReceivedEventHandler<FriendRequestUnionEvent>? FriendRequest;
    public event ReceivedEventHandler<GroupJoinRequestUnionEvent>? GroupJoinRequest;
    public event ReceivedEventHandler<GroupInvitedJoinRequestUnionEvent>? GroupInvitedJoinRequest;
    public event ReceivedEventHandler<GroupInvitationUnionEvent>? GroupInvitation;
    public event ReceivedEventHandler<FriendNudgeUnionEvent>? FriendNudge;
    public event ReceivedEventHandler<FriendFileUploadUnionEvent>? FriendFileUpload;
    public event ReceivedEventHandler<GroupAdminChangeUnionEvent>? GroupAdminChange;
    public event ReceivedEventHandler<GroupEssenceMessageChangeUnionEvent>? GroupEssenceMessageChange;
    public event ReceivedEventHandler<GroupMemberIncreaseUnionEvent>? GroupMemberIncrease;
    public event ReceivedEventHandler<GroupMemberDecreaseUnionEvent>? GroupMemberDecrease;
    public event ReceivedEventHandler<GroupNameChangeUnionEvent>? GroupNameChange;
    public event ReceivedEventHandler<GroupMessageReactionUnionEvent>? GroupMessageReaction;
    public event ReceivedEventHandler<GroupMuteUnionEvent>? GroupMute;
    public event ReceivedEventHandler<GroupWholeMuteUnionEvent>? GroupWholeMute;
    public event ReceivedEventHandler<GroupNudgeUnionEvent>? GroupNudge;
    public event ReceivedEventHandler<GroupFileUploadUnionEvent>? GroupFileUpload;

    public void Received(Event @event)
    {
        switch (@event)
        {
            case BotOfflineUnionEvent typedEvent:
                BotOffline?.Invoke(typedEvent);
                break;
            case MessageReceiveUnionEvent typedEvent:
                IncomingMessage?.Invoke(typedEvent);
                break;
            case MessageRecallUnionEvent typedEvent:
                MessageRecall?.Invoke(typedEvent);
                break;
            case FriendRequestUnionEvent typedEvent:
                FriendRequest?.Invoke(typedEvent);
                break;
            case GroupJoinRequestUnionEvent typedEvent:
                GroupJoinRequest?.Invoke(typedEvent);
                break;
            case GroupInvitedJoinRequestUnionEvent typedEvent:
                GroupInvitedJoinRequest?.Invoke(typedEvent);
                break;
            case GroupInvitationUnionEvent typedEvent:
                GroupInvitation?.Invoke(typedEvent);
                break;
            case FriendNudgeUnionEvent typedEvent:
                FriendNudge?.Invoke(typedEvent);
                break;
            case FriendFileUploadUnionEvent typedEvent:
                FriendFileUpload?.Invoke(typedEvent);
                break;
            case GroupAdminChangeUnionEvent typedEvent:
                GroupAdminChange?.Invoke(typedEvent);
                break;
            case GroupEssenceMessageChangeUnionEvent typedEvent:
                GroupEssenceMessageChange?.Invoke(typedEvent);
                break;
            case GroupMemberIncreaseUnionEvent typedEvent:
                GroupMemberIncrease?.Invoke(typedEvent);
                break;
            case GroupMemberDecreaseUnionEvent typedEvent:
                GroupMemberDecrease?.Invoke(typedEvent);
                break;
            case GroupNameChangeUnionEvent typedEvent:
                GroupNameChange?.Invoke(typedEvent);
                break;
            case GroupMessageReactionUnionEvent typedEvent:
                GroupMessageReaction?.Invoke(typedEvent);
                break;
            case GroupMuteUnionEvent typedEvent:
                GroupMute?.Invoke(typedEvent);
                break;
            case GroupWholeMuteUnionEvent typedEvent:
                GroupWholeMute?.Invoke(typedEvent);
                break;
            case GroupNudgeUnionEvent typedEvent:
                GroupNudge?.Invoke(typedEvent);
                break;
            case GroupFileUploadUnionEvent typedEvent:
                GroupFileUpload?.Invoke(typedEvent);
                break;
        }
    }
}
