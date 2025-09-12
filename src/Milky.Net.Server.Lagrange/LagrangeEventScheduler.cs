using System.Text.Json.Serialization.Metadata;

using Lagrange.Core;
using Lagrange.Core.Event.EventArg;
using Lagrange.Core.Message;
using Lagrange.Core.Message.Entity;

using Milky.Net.Model;

using Lagrange = Lagrange.Core.Event.EventArg;

namespace Milky.Net.Server.Lagrange;

public sealed class LagrangeEventScheduler : IDisposable
{
    private readonly BotContext _bot;
    public event Func<object, JsonTypeInfo, Task>? OnReceived;

    public LagrangeEventScheduler(BotContext bot)
    {
        _bot = bot;
        _bot.Invoker.OnBotOnlineEvent += BotOnlineEvent;
        _bot.Invoker.OnBotOfflineEvent += BotOfflineEventAsync;
        _bot.Invoker.OnBotLogEvent += BotLogEvent;
        _bot.Invoker.OnBotCaptchaEvent += BotCaptchaEvent;
        _bot.Invoker.OnBotNewDeviceVerify += BotNewDeviceVerify;
        _bot.Invoker.OnGroupInvitationReceived += GroupInvitationReceived;
        _bot.Invoker.OnFriendMessageReceived += FriendMessageReceived;
        _bot.Invoker.OnGroupMessageReceived += GroupMessageReceived;
        _bot.Invoker.OnTempMessageReceived += TempMessageReceived;
        _bot.Invoker.OnGroupAdminChangedEvent += GroupAdminChangedEvent;
        _bot.Invoker.OnGroupMemberIncreaseEvent += GroupMemberIncreaseEvent;
        _bot.Invoker.OnGroupMemberDecreaseEvent += GroupMemberDecreaseEvent;
        _bot.Invoker.OnFriendRequestEvent += FriendRequestEvent;
        _bot.Invoker.OnGroupInvitationRequestEvent += GroupInvitationRequestEvent;
        _bot.Invoker.OnGroupJoinRequestEvent += GroupJoinRequestEvent;
        _bot.Invoker.OnGroupMuteEvent += GroupMuteEvent;
        _bot.Invoker.OnGroupMemberMuteEvent += GroupMemberMuteEvent;
        _bot.Invoker.OnGroupRecallEvent += GroupRecallEvent;
        _bot.Invoker.OnFriendRecallEvent += FriendRecallEvent;
        _bot.Invoker.OnDeviceLoginEvent += DeviceLoginEvent;
        _bot.Invoker.OnFriendPokeEvent += FriendPokeEvent;
        _bot.Invoker.OnGroupPokeEvent += GroupPokeEvent;
        _bot.Invoker.OnGroupEssenceEvent += GroupEssenceEvent;
        _bot.Invoker.OnGroupReactionEvent += GroupReactionEvent;
        _bot.Invoker.OnGroupNameChangeEvent += GroupNameChangeEvent;
    }

    private async Task WriteDataAsync<T>(T data, JsonTypeInfo<T> jsonTypeInfo)
        where T : notnull
        => await (OnReceived?.Invoke(data, jsonTypeInfo) ?? Task.CompletedTask);

    private static IEnumerable<IncomingSegment> Convert(IEnumerable<IMessageEntity> messages)
    {
        foreach (var message in messages)
        {
            switch (message)
            {
                case TextEntity text:
                    yield return new TextIncomingSegment(new(text.Text));
                    break;
                case MentionEntity mention:
                    yield return new MentionIncomingSegment(new(mention.Uin));
                    break;
                case FaceEntity face:
                    yield return new FaceIncomingSegment(new(face.FaceId.ToString()));
                    break;
                case ImageEntity image:
                    yield return new ImageIncomingSegment(new(System.Convert.ToHexString(image.ImageMd5), image.ImageUrl, (int)image.PictureSize.X, (int)image.PictureSize.Y, string.Empty, image.SubType switch
                    {
                        1 => SubType.Sticker,
                        _ => SubType.Normal,
                    }));
                    break;
                case RecordEntity record:
                    yield return new RecordIncomingSegment(new(System.Convert.ToHexString(record.AudioMd5), record.AudioUrl, record.AudioLength));
                    break;
                case VideoEntity video:
                    yield return new VideoIncomingSegment(new(video.VideoHash, video.VideoUrl, (int)video.Size.X, (int)video.Size.Y, video.VideoLength));
                    break;
                case ForwardEntity forward:
                    yield return new ForwardIncomingSegment(new(forward.MessageId.ToString()));
                    break;
                case LightAppEntity lightApp:
                    yield return new LightAppIncomingSegment(new(lightApp.AppName, lightApp.Payload));
                    break;
                case XmlEntity xml:
                    yield return new XmlIncomingSegment(new(-1, xml.Xml));
                    break;
            }
        }
    }

    private void BotOnlineEvent(BotContext bot, Lagrange::BotOnlineEvent eventArgs) { }

    private async void BotOfflineEventAsync(BotContext bot, Lagrange::BotOfflineEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.Message)),
            MilkyJsonSerializerContext.Default.EventBotOfflineEvent);

    private void BotLogEvent(BotContext bot, Lagrange::BotLogEvent eventArgs) { }

    private void BotCaptchaEvent(BotContext bot, Lagrange::BotCaptchaEvent eventArgs) { }

    private void BotNewDeviceVerify(BotContext bot, BotNewDeviceVerifyEvent eventArgs) { }

    private async void GroupInvitationReceived(BotContext bot, Lagrange::GroupInvitationEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.GroupUin, -1, eventArgs.InvitorUin)),
            MilkyJsonSerializerContext.Default.EventGroupInvitationEvent);

    private async void FriendMessageReceived(BotContext bot, FriendMessageEvent eventArgs)
    {

        await WriteDataAsync(
                new(
                    eventArgs.EventTime,
                    bot.BotUin,
                    new FriendIncomingMessage(
                        eventArgs.Chain.FriendUin,
                        eventArgs.Chain.Sequence,
                        eventArgs.Chain.TargetUin,
                        eventArgs.Chain.Time,
                        [.. Convert(eventArgs.Chain)],
                        new(-1, string.Empty, Sex.Unknown, string.Empty, string.Empty, new(-1, string.Empty))
                        )),
                MilkyJsonSerializerContext.Default.EventIncomingMessage);
    }

    private async void GroupMessageReceived(BotContext bot, GroupMessageEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new GroupIncomingMessage(
                    eventArgs.Chain.FriendUin,
                    eventArgs.Chain.Sequence,
                    eventArgs.Chain.TargetUin,
                    eventArgs.Chain.Time,
                    [.. Convert(eventArgs.Chain)],
                    new(-1, string.Empty, -1, -1),
                        new(-1, string.Empty, Sex.Unknown, -1, string.Empty, string.Empty, -1, Role.Member, DateTimeOffset.UnixEpoch, DateTimeOffset.UnixEpoch, DateTimeOffset.UnixEpoch)
                    )),
            MilkyJsonSerializerContext.Default.EventIncomingMessage);

    private async void TempMessageReceived(BotContext bot, TempMessageEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new TempIncomingMessage(
                    eventArgs.Chain.FriendUin,
                    eventArgs.Chain.Sequence,
                    eventArgs.Chain.TargetUin,
                    eventArgs.Chain.Time,
                    [.. Convert(eventArgs.Chain)],
                    null)),
            MilkyJsonSerializerContext.Default.EventIncomingMessage);

    private async void GroupAdminChangedEvent(BotContext bot, Lagrange::GroupAdminChangedEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.GroupUin, eventArgs.AdminUin, eventArgs.IsPromote)),
            MilkyJsonSerializerContext.Default.EventGroupAdminChangeEvent);

    private async void GroupMemberIncreaseEvent(BotContext bot, Lagrange::GroupMemberIncreaseEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.GroupUin, eventArgs.MemberUin, null, eventArgs.InvitorUin)),
            MilkyJsonSerializerContext.Default.EventGroupMemberIncreaseEvent);

    private async void GroupMemberDecreaseEvent(BotContext bot, Lagrange::GroupMemberDecreaseEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.GroupUin, eventArgs.MemberUin, eventArgs.OperatorUin)),
            MilkyJsonSerializerContext.Default.EventGroupMemberDecreaseEvent);

    private async void FriendRequestEvent(BotContext bot, Lagrange::FriendRequestEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.SourceUin, string.Empty, eventArgs.Message, eventArgs.Source)),
            MilkyJsonSerializerContext.Default.EventFriendRequestEvent);

    private async void GroupInvitationRequestEvent(BotContext bot, Lagrange::GroupInvitationRequestEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.GroupUin, -1, eventArgs.InvitorUin)),
            MilkyJsonSerializerContext.Default.EventGroupInvitationEvent);

    private async void GroupJoinRequestEvent(BotContext bot, Lagrange::GroupJoinRequestEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.GroupUin, -1, false, eventArgs.TargetUin, string.Empty)),
            MilkyJsonSerializerContext.Default.EventGroupJoinRequestEvent);

    private void GroupMuteEvent(BotContext bot, Lagrange::GroupMuteEvent eventArgs) { }

    private async void GroupMemberMuteEvent(BotContext bot, Lagrange::GroupMemberMuteEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.GroupUin, eventArgs.TargetUin, eventArgs.OperatorUin ?? 0, (int)eventArgs.Duration)),
            MilkyJsonSerializerContext.Default.EventGroupMuteEvent);

    private void GroupRecallEvent(BotContext bot, Lagrange::GroupRecallEvent eventArgs) { }

    private void FriendRecallEvent(BotContext bot, Lagrange::FriendRecallEvent eventArgs) { }

    private void DeviceLoginEvent(BotContext bot, Lagrange::DeviceLoginEvent eventArgs) { }

    private async void FriendPokeEvent(BotContext bot, Lagrange::FriendPokeEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(
                    eventArgs.TargetUin,
                    eventArgs.OperatorUin == bot.BotUin,
                    eventArgs.OperatorUin != bot.BotUin,
                    eventArgs.Action,
                    eventArgs.Suffix,
                    eventArgs.ActionImgUrl)),
            MilkyJsonSerializerContext.Default.EventFriendNudgeEvent);

    private async void GroupPokeEvent(BotContext bot, Lagrange::GroupPokeEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(
                    eventArgs.GroupUin,
                    eventArgs.OperatorUin,
                    eventArgs.TargetUin,
                    eventArgs.Action,
                    eventArgs.Suffix,
                    eventArgs.ActionImgUrl)),
            MilkyJsonSerializerContext.Default.EventGroupNudgeEvent);

    private async void GroupEssenceEvent(BotContext bot, Lagrange::GroupEssenceEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.GroupUin, eventArgs.Sequence, eventArgs.IsSet)),
            MilkyJsonSerializerContext.Default.EventGroupEssenceMessageChangeEvent);

    private void GroupReactionEvent(BotContext bot, Lagrange::GroupReactionEvent eventArgs) { }

    private async void GroupNameChangeEvent(BotContext bot, Lagrange::GroupNameChangeEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.GroupUin, eventArgs.Name, -1)),
            MilkyJsonSerializerContext.Default.EventGroupNameChangeEvent);
    public void Dispose()
    {
        _bot.Invoker.OnBotOnlineEvent -= BotOnlineEvent;
        _bot.Invoker.OnBotOfflineEvent -= BotOfflineEventAsync;
        _bot.Invoker.OnBotLogEvent -= BotLogEvent;
        _bot.Invoker.OnBotCaptchaEvent -= BotCaptchaEvent;
        _bot.Invoker.OnBotNewDeviceVerify -= BotNewDeviceVerify;
        _bot.Invoker.OnGroupInvitationReceived -= GroupInvitationReceived;
        _bot.Invoker.OnFriendMessageReceived -= FriendMessageReceived;
        _bot.Invoker.OnGroupMessageReceived -= GroupMessageReceived;
        _bot.Invoker.OnTempMessageReceived -= TempMessageReceived;
        _bot.Invoker.OnGroupAdminChangedEvent -= GroupAdminChangedEvent;
        _bot.Invoker.OnGroupMemberIncreaseEvent -= GroupMemberIncreaseEvent;
        _bot.Invoker.OnGroupMemberDecreaseEvent -= GroupMemberDecreaseEvent;
        _bot.Invoker.OnFriendRequestEvent -= FriendRequestEvent;
        _bot.Invoker.OnGroupInvitationRequestEvent -= GroupInvitationRequestEvent;
        _bot.Invoker.OnGroupJoinRequestEvent -= GroupJoinRequestEvent;
        _bot.Invoker.OnGroupMuteEvent -= GroupMuteEvent;
        _bot.Invoker.OnGroupMemberMuteEvent -= GroupMemberMuteEvent;
        _bot.Invoker.OnGroupRecallEvent -= GroupRecallEvent;
        _bot.Invoker.OnFriendRecallEvent -= FriendRecallEvent;
        _bot.Invoker.OnDeviceLoginEvent -= DeviceLoginEvent;
        _bot.Invoker.OnFriendPokeEvent -= FriendPokeEvent;
        _bot.Invoker.OnGroupPokeEvent -= GroupPokeEvent;
        _bot.Invoker.OnGroupEssenceEvent -= GroupEssenceEvent;
        _bot.Invoker.OnGroupReactionEvent -= GroupReactionEvent;
        _bot.Invoker.OnGroupNameChangeEvent -= GroupNameChangeEvent;
    }
}
