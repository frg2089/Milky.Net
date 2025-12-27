namespace Milky.Net.Client;

/// <summary>
/// 默认的事件调度器提供程序
/// </summary>
public sealed class MilkyEventSchedulerProvider : IMilkyEventSchedulerProvider
{
    private static readonly Lazy<MilkyEventSchedulerProvider> Lazy = new(() => new());
    public static IMilkyEventSchedulerProvider Default => Lazy.Value;

    public IMilkyEventScheduler Create(MilkyClient client)
        => new MilkyEventScheduler(client);
}