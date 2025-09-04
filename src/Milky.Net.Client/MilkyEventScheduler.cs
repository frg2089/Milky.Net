using Milky.Net.Model;

namespace Milky.Net.Client;

/// <summary>
/// 事件调度器
/// </summary>
public sealed partial class MilkyEventScheduler
{
    private readonly MilkyClient _client;
    internal MilkyEventScheduler(MilkyClient client)
        => _client = client;

    internal partial void Received(Event @event);
}
