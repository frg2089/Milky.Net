using Lagrange.Core.Common;
using Lagrange.Core.Common.Entity;

using Shimakaze.Milky.Model;

namespace Shimakaze.Milky.Server.Lagrange;
public static class LagrangeConverters
{
    public static Sex Convert(this BotUserInfo.GenderInfo gender) => gender switch
    {
        BotUserInfo.GenderInfo.Male => Sex.Male,
        BotUserInfo.GenderInfo.Female => Sex.Female,
        _ => Sex.Unknown
    };
    public static QQProtocolType Convert(this Protocols protocols) => protocols switch
    {
        Protocols.Windows => QQProtocolType.Windows,
        Protocols.MacOs => QQProtocolType.MacOS,
        Protocols.Linux => QQProtocolType.Linux,
        _ => throw new NotSupportedException(),
    };
    public static Role Convert(this GroupMemberPermission permission) => permission switch
    {
        GroupMemberPermission.Owner => Role.Owner,
        GroupMemberPermission.Admin => Role.Administrator,
        GroupMemberPermission.Member => Role.Member,
        _ => throw new NotSupportedException(),
    };
}
