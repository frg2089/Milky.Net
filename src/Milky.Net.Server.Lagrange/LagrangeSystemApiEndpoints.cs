using System.Diagnostics;
using System.Reflection;

using Lagrange.Core;
using Lagrange.Core.Common.Interface.Api;

using Milky.Net.Model;

namespace Milky.Net.Server.Lagrange;

public class LagrangeSystemApiEndpoints(BotContext bot) : IMilkySystemApiEndpoints
{
    public async Task<GetCookiesOutput> GetCookiesAsync(GetCookiesInput input, CancellationToken cancellationToken = default)
    {
        var result = await bot.FetchCookies([input.Domain]);
        return new(string.Join("; ", result));
    }

    public Task<GetCSRFTokenOutput> GetCsrfTokenAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<GetFriendInfoOutput> GetFriendInfoAsync(GetFriendInfoInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public async Task<GetFriendListOutput> GetFriendListAsync(GetFriendListInput input, CancellationToken cancellationToken = default)
    {
        var list = await bot.FetchFriends(input.NoCache);
        var result = list.Select(i => new FriendEntity(
            i.Uin,
            i.Nickname,
            Sex.Unknown,
            i.Qid,
            i.Remarks,
            new((int)i.Group.GroupId, i.Group.GroupName)
            ));

        return new([.. result]);
    }

    public Task<GetGroupInfoOutput> GetGroupInfoAsync(GetGroupInfoInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public async Task<GetGroupListOutput> GetGroupListAsync(GetGroupListInput input, CancellationToken cancellationToken = default)
    {
        var list = await bot.FetchGroups(input.NoCache);
        var result = list.Select(i => new GroupEntity(
            i.GroupUin,
            i.GroupName,
            (int)i.MemberCount,
            (int)i.MaxMember
            ));

        return new([.. result]);
    }

    public Task<GetGroupMemberInfoOutput> GetGroupMemberInfoAsync(GetGroupMemberInfoInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public async Task<GetGroupMemberListOutput> GetGroupMemberListAsync(GetGroupMemberListInput input, CancellationToken cancellationToken = default)
    {
        var list = await bot.FetchMembers((uint)input.GroupId, input.NoCache);
        var result = list.Select(i => new GroupMemberEntity(
            i.Uin,
            string.Empty,
            Sex.Unknown,
            input.GroupId,
            i.MemberName,
            i.SpecialTitle ?? string.Empty,
            (int)i.GroupLevel,
            i.Permission.Convert(),
            i.JoinTime,
            i.LastMsgTime,
            i.ShutUpTimestamp
            ));

        return new([.. result]);
    }

    public Task<GetImplInfoOutput> GetImplInfoAsync(CancellationToken cancellationToken = default)
    {
        var asm = typeof(LagrangeSystemApiEndpoints).Assembly;
        return Task.FromResult(new GetImplInfoOutput(
            asm.GetName().Name ?? "Milky.Net.Server.Lagrange",
            asm.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "Unknown",
            bot.AppInfo.CurrentVersion,
            bot.Config.Protocol.Convert(),
            asm.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(i => i.Key is "MilkyVersion")?.Value ?? "Unknown"));
    }

    public Task<GetLoginInfoOutput> GetLoginInfoAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(new GetLoginInfoOutput(bot.BotUin, bot.BotName ?? string.Empty));

    public async Task<GetUserProfileOutput> GetUserProfileAsync(GetUserProfileInput input, CancellationToken cancellationToken = default)
    {
        var result = await bot.FetchUserInfo((uint)input.UserId);
        Debug.Assert(result is not null);
        return new(
            result.Nickname,
            result.Qid ?? string.Empty,
            (int)result.Age,
            result.Gender.Convert(),
            string.Empty, // TODO: 获取备注
            string.Empty, // TODO: 获取个性签名
            (int)result.Level,
            result.Country,
            result.City,
            result.School);
    }
}