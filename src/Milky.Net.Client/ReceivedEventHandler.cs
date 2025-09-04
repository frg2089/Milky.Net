using Milky.Net.Model;

namespace Milky.Net.Client;

/// <summary>
/// 事件处理程序
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="event"></param>
public delegate void ReceivedEventHandler<T>(MilkyClient client, T @event) where T : Event;
