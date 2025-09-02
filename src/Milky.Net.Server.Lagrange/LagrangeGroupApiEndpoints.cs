using Lagrange.Core;
using Lagrange.Core.Common.Interface.Api;

using Milky.Net.Model;

namespace Milky.Net.Server.Lagrange;

public class LagrangeGroupApiEndpoints(BotContext bot) : IMilkyGroupApiEndpoints
{
    public Task AcceptGroupInvitationAsync(AcceptGroupInvitationInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task AcceptGroupRequestAsync(AcceptGroupRequestInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task DeleteGroupAnnouncementAsync(DeleteGroupAnnouncementInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<GetGroupAnnouncementListOutput> GetGroupAnnouncementListAsync(GetGroupAnnouncementListInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<GetGroupEssenceMessagesOutput> GetGroupEssenceMessagesAsync(GetGroupEssenceMessagesInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<GetGroupNotificationsOutput> GetGroupNotificationsAsync(GetGroupNotificationsInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task KickGroupMemberAsync(KickGroupMemberInput input, CancellationToken cancellationToken = default)
        => bot.KickGroupMember((uint)input.GroupId, (uint)input.UserId, input.RejectAddRequest);

    public Task QuitGroupAsync(QuitGroupInput input, CancellationToken cancellationToken = default)
        => bot.LeaveGroup((uint)input.GroupId);

    public Task RejectGroupInvitationAsync(RejectGroupInvitationInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task RejectGroupRequestAsync(RejectGroupRequestInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task SendGroupAnnouncementAsync(SendGroupAnnouncementInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task SendGroupMessageReactionAsync(SendGroupMessageReactionInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task SendGroupNudgeAsync(SendGroupNudgeInput input, CancellationToken cancellationToken = default)
        => bot.GroupPoke((uint)input.GroupId, (uint)input.UserId);

    public Task SetGroupAvatarAsync(SetGroupAvatarInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task SetGroupEssenceMessageAsync(SetGroupEssenceMessageInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task SetGroupMemberAdminAsync(SetGroupMemberAdminInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task SetGroupMemberCardAsync(SetGroupMemberCardInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task SetGroupMemberMuteAsync(SetGroupMemberMuteInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task SetGroupMemberSpecialTitleAsync(SetGroupMemberSpecialTitleInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task SetGroupNameAsync(SetGroupNameInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task SetGroupWholeMuteAsync(SetGroupWholeMuteInput input, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
