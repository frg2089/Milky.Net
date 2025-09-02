using Lagrange.Core;
using Lagrange.Core.Common.Interface.Api;
using Lagrange.Core.Message;

using Milky.Net.Model;

namespace Milky.Net.Server.Lagrange;

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

    async Task<SendGroupMessageOutput> IMilkyMessageApiEndpoints.SendPrivateMessageAsync(SendPrivateMessageInput input, CancellationToken cancellationToken)
    {
        var tmp = await SendPrivateMessageAsync(input, cancellationToken);
        return new(tmp.MessageSeq, tmp.Time);
    }


    private async Task<MessageResult> SendMessageAsync(MessageBuilder builder, OutgoingSegment[] segments, CancellationToken cancellationToken = default)
    {
        foreach (var item in segments)
        {
            switch (item)
            {
                case TextUnionOutgoingSegment text:
                    builder.Text(text.Data.Text);
                    break;
                case MentionUnionOutgoingSegment mention:
                    builder.Mention((uint)mention.Data.UserId);
                    break;
                case MentionAllUnionOutgoingSegment mention_all:
                    break;
                case FaceUnionOutgoingSegment face:
                    break;
                case ReplyUnionOutgoingSegment reply:
                    break;
                case ImageUnionOutgoingSegment image:
                    {
                        var path = Path.GetTempFileName();
                        await using var fs = await fileFetcher.FetchFileAsync(image.Data.Uri, cancellationToken);
                        await using var tmpfs = File.Create(path);
                        await fs.CopyToAsync(tmpfs, cancellationToken);

                        builder.Image(path);
                    }
                    break;
                case RecordUnionOutgoingSegment record:
                    break;
                case VideoUnionOutgoingSegment video:
                    break;
                case ForwardUnionOutgoingSegment forward:
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        return await bot.SendMessage(builder.Build());
    }

}
