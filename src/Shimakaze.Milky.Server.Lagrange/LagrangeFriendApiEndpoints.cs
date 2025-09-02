using Lagrange.Core;
using Lagrange.Core.Common.Interface.Api;

using Shimakaze.Milky.Model;

namespace Shimakaze.Milky.Server.Lagrange;

public class LagrangeFriendApiEndpoints(BotContext bot) : IMilkyFriendApiEndpoints
{
    public Task AcceptFriendRequestAsync(AcceptFriendRequestInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<GetFriendRequestsOutput> GetFriendRequestsAsync(GetFriendRequestsInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task RejectFriendRequestAsync(RejectFriendRequestInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public async Task SendFriendNudgeAsync(SendFriendNudgeInput input, CancellationToken cancellationToken = default)
        => await bot.FriendPoke((uint)(input.IsSelf ? bot.BotUin : input.UserId));

    public async Task SendProfileLikeAsync(SendProfileLikeInput input, CancellationToken cancellationToken = default)
        => await bot.Like((uint)input.UserId, (uint)input.Count);
}
