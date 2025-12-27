namespace Milky.Net.Client;

/// <summary>
/// 事件调度器提供程序
/// </summary>
public interface IMilkyEventSchedulerProvider
{
    /// <summary>
    /// 创建事件调度器
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    IMilkyEventScheduler Create(MilkyClient client);
}