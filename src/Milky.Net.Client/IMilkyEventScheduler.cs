using Milky.Net.Model;

namespace Milky.Net.Client;

/// <summary>
/// 事件调度器
/// </summary>
public partial interface IMilkyEventScheduler
{
    void Received(Event @event);
}