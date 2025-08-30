using System.Threading;

using Lagrange.Core;
using Lagrange.Core.Common.Interface.Api;
using Lagrange.Core.Message;

using Shimakaze.Milky.Model.Api.Message;
using Shimakaze.Milky.Model.Message;

namespace Shimakaze.Milky.Server.Lagrange;

public class LagrangeMessageApiEndpoints(BotContext bot, IFileFetcher fileFetcher) : IMilkyMessageApiEndpoints
{
    public Task<GetForwardedMessagesOutput> GetForwardedMessagesAsync(GetForwardedMessagesInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<GetHistoryMessagesOutput> GetHistoryMessagesAsync(GetHistoryMessagesInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<GetMessageOutput> GetMessageAsync(GetMessageInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<GetResourceTempUrlOutput> GetResourceTempUrlAsync(GetResourceTempUrlInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task MarkMessageAsReadAsync(MarkMessageAsReadInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public async Task RecallGroupMessageAsync(RecallGroupMessageInput input, CancellationToken cancellationToken = default)
        => await bot.RecallGroupMessage((uint)input.GroupId, (uint)input.MessageSeq);

    public Task RecallPrivateMessageAsync(RecallPrivateMessageInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public async Task<SendGroupMessageOutput> SendGroupMessageAsync(SendGroupMessageInput input, CancellationToken cancellationToken = default)
    {
        var builder = MessageBuilder.Group((uint)input.GroupId);

        var result = await SendMessageAsync(builder, input.Message, cancellationToken);

        return new(
            result.Sequence ?? result.ClientSequence,
            DateTimeOffset.FromUnixTimeSeconds(result.Timestamp));
    }

    public async Task<SendPrivateMessageOutput> SendPrivateMessageAsync(SendPrivateMessageInput input, CancellationToken cancellationToken = default)
    {
        var builder = MessageBuilder.Friend((uint)input.UserId);

        var result = await SendMessageAsync(builder, input.Message, cancellationToken);

        return new(
            result.Sequence ?? result.ClientSequence,
            DateTimeOffset.FromUnixTimeSeconds(result.Timestamp));
    }


    private async Task<MessageResult> SendMessageAsync(MessageBuilder builder, OutgoingSegment[] segments, CancellationToken cancellationToken = default)
    {
        foreach (var item in segments)
        {
            switch (item)
            {
                case OutgoingSegment<TextSegmentData> text:
                    builder.Text(text.Data.Text);
                    break;
                case OutgoingSegment<MentionSegmentData> mention:
                    builder.Mention((uint)mention.Data.UserId);
                    break;
                case OutgoingSegment<object> mention_all:
                    break;
                case OutgoingSegment<FaceSegmentData> face:
                    break;
                case OutgoingSegment<ReplySegmentData> reply:
                    break;
                case OutgoingSegment<ImageSegmentData> image:
                    {
                        var path = Path.GetTempFileName();
                        await using var fs = await fileFetcher.FetchFileAsync(image.Data.Uri, cancellationToken);
                        await using var tmpfs = File.Create(path);
                        await fs.CopyToAsync(tmpfs, cancellationToken);

                        builder.Image(path);
                    }
                    break;
                case OutgoingSegment<OutgoingResourceSegmentBase> record:
                    break;
                case OutgoingSegment<VideoSegmentData> video:
                    break;
                case OutgoingSegment<ForwardSegmentData> forward:
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        return await bot.SendMessage(builder.Build());
    }
}
