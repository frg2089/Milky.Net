using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

using Lagrange.Core;
using Lagrange.Core.Event.EventArg;
using Lagrange.Core.Message;
using Lagrange.Core.Message.Entity;

using Shimakaze.Milky.Model;
using Shimakaze.Milky.Model.Message;

namespace Shimakaze.Milky.Server.Lagrange;

public sealed class LagrangeEventManager : IDisposable
{
    private readonly BotContext _bot;
    private readonly HttpContext _context;
    private readonly CancellationToken _cancellationToken;
    public LagrangeEventManager(BotContext bot, HttpContext context, CancellationToken cancellationToken = default)
    {
        _bot = bot;
        _context = context;
        _cancellationToken = cancellationToken;
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


        context.Response.StatusCode = StatusCodes.Status206PartialContent;
        context.Response.ContentType = MediaTypeNames.Text.EventStream;
        context.Response.Headers.CacheControl = "no-cache";
        context.Response.Headers.Connection = "keep-alive";
    }

    private async Task WriteDataAsync<T>(T data, JsonTypeInfo<T> jsonTypeInfo)
    {
        await using MemoryStream ms = new();
        await JsonSerializer.SerializeAsync(ms, data, jsonTypeInfo, _cancellationToken);
        await ms.FlushAsync(_cancellationToken);
        ms.Seek(0, SeekOrigin.Begin);
        using StreamReader sr = new(ms);
        while (!sr.EndOfStream)
        {
            var line = await sr.ReadLineAsync(_cancellationToken);
            if (string.IsNullOrEmpty(line))
                break;

            await _context.Response.WriteAsync($"data: {line}\n", _cancellationToken);
        }
        await _context.Response.WriteAsync($"\n", _cancellationToken);
        await _context.Response.Body.FlushAsync(_cancellationToken);
    }

    private IEnumerable<IncomingSegment> Convert(IEnumerable<IMessageEntity> messages)
    {
        foreach (var message in messages)
        {
            switch (message)
            {
                case TextEntity text:
                    yield return new IncomingSegment<IncomingTextSegmentData>(new(text.Text));
                    break;
                case MentionEntity mention:
                    yield return new IncomingSegment<IncomingMentionSegmentData>(new(mention.Uin));
                    break;
                case FaceEntity face:
                    yield return new IncomingSegment<IncomingFaceSegmentData>(new(face.FaceId.ToString()));
                    break;
                case ImageEntity image:
                    yield return new IncomingSegment<IncomingImageSegmentData>(new(System.Convert.ToHexString(image.ImageMd5), image.ImageUrl, (int)image.PictureSize.X, (int)image.PictureSize.Y, string.Empty, image.SubType.ToString()));
                    break;
                case RecordEntity record:
                    yield return new IncomingSegment<IncomingRecordSegmentData>(new(System.Convert.ToHexString(record.AudioMd5), record.AudioUrl, record.AudioLength));
                    break;
                case VideoEntity video:
                    yield return new IncomingSegment<IncomingVideoSegmentData>(new(video.VideoHash, video.VideoUrl, (int)video.Size.X, (int)video.Size.Y, video.VideoLength));
                    break;
                case ForwardEntity forward:
                    yield return new IncomingSegment<IncomingForwardSegmentData>(new(forward.MessageId.ToString()));
                    break;
                case LightAppEntity lightApp:
                    yield return new IncomingSegment<IncomingLightAppSegmentData>(new(lightApp.AppName, lightApp.Payload));
                    break;
                case XmlEntity xml:
                    yield return new IncomingSegment<IncomingXmlSegmentData>(new(-1, xml.Xml));
                    break;
            }
        }
    }

    private void BotOnlineEvent(BotContext bot, BotOnlineEvent eventArgs) { }

    private async void BotOfflineEventAsync(BotContext bot, BotOfflineEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.Message)),
            MilkyJsonSerializerContext.Default.EventBotOfflineEvent);

    private void BotLogEvent(BotContext bot, BotLogEvent eventArgs) { }

    private void BotCaptchaEvent(BotContext bot, BotCaptchaEvent eventArgs) { }

    private void BotNewDeviceVerify(BotContext bot, BotNewDeviceVerifyEvent eventArgs) { }

    private async void GroupInvitationReceived(BotContext bot, GroupInvitationEvent eventArgs)
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
                    new FriendMessage(
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
                new GroupMessage(
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
                new TempMessage(
                    eventArgs.Chain.FriendUin,
                    eventArgs.Chain.Sequence,
                    eventArgs.Chain.TargetUin,
                    eventArgs.Chain.Time,
                    [.. Convert(eventArgs.Chain)])),
            MilkyJsonSerializerContext.Default.EventIncomingMessage);

    private async void GroupAdminChangedEvent(BotContext bot, GroupAdminChangedEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.GroupUin, eventArgs.AdminUin, eventArgs.IsPromote)),
            MilkyJsonSerializerContext.Default.EventGroupAdminChangeEvent);

    private async void GroupMemberIncreaseEvent(BotContext bot, GroupMemberIncreaseEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.GroupUin, eventArgs.MemberUin, null, eventArgs.InvitorUin)),
            MilkyJsonSerializerContext.Default.EventGroupMemberIncreaseEvent);

    private async void GroupMemberDecreaseEvent(BotContext bot, GroupMemberDecreaseEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.GroupUin, eventArgs.MemberUin, eventArgs.OperatorUin)),
            MilkyJsonSerializerContext.Default.EventGroupMemberDecreaseEvent);

    private async void FriendRequestEvent(BotContext bot, FriendRequestEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.SourceUin, string.Empty, eventArgs.Message, eventArgs.Source)),
            MilkyJsonSerializerContext.Default.EventFriendRequestEvent);

    private async void GroupInvitationRequestEvent(BotContext bot, GroupInvitationRequestEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.GroupUin, -1, eventArgs.InvitorUin)),
            MilkyJsonSerializerContext.Default.EventGroupInvitationEvent);

    private async void GroupJoinRequestEvent(BotContext bot, GroupJoinRequestEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.GroupUin, -1, false, eventArgs.TargetUin, string.Empty)),
            MilkyJsonSerializerContext.Default.EventGroupJoinRequestEvent);

    private void GroupMuteEvent(BotContext bot, GroupMuteEvent eventArgs) { }

    private async void GroupMemberMuteEvent(BotContext bot, GroupMemberMuteEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.GroupUin, eventArgs.TargetUin, eventArgs.OperatorUin ?? 0, (int)eventArgs.Duration)),
            MilkyJsonSerializerContext.Default.EventGroupMuteEvent);

    private void GroupRecallEvent(BotContext bot, GroupRecallEvent eventArgs) { }

    private void FriendRecallEvent(BotContext bot, FriendRecallEvent eventArgs) { }

    private void DeviceLoginEvent(BotContext bot, DeviceLoginEvent eventArgs) { }

    private async void FriendPokeEvent(BotContext bot, FriendPokeEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.TargetUin, eventArgs.OperatorUin == bot.BotUin, eventArgs.OperatorUin != bot.BotUin)),
            MilkyJsonSerializerContext.Default.EventFriendNudgeEvent);

    private async void GroupPokeEvent(BotContext bot, GroupPokeEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.GroupUin, eventArgs.OperatorUin, eventArgs.TargetUin)),
            MilkyJsonSerializerContext.Default.EventGroupNudgeEvent);

    private async void GroupEssenceEvent(BotContext bot, GroupEssenceEvent eventArgs)
        => await WriteDataAsync(
            new(
                eventArgs.EventTime,
                bot.BotUin,
                new(eventArgs.GroupUin, eventArgs.Sequence, eventArgs.IsSet)),
            MilkyJsonSerializerContext.Default.EventGroupEssenceMessageChangeEvent);

    private void GroupReactionEvent(BotContext bot, GroupReactionEvent eventArgs) { }

    private async void GroupNameChangeEvent(BotContext bot, GroupNameChangeEvent eventArgs)
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
