using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Event;


/// <summary>
/// 机器人离线事件
/// </summary>
/// <param name="Reason">下线原因</param>
public sealed record class BotOfflineEvent(
    [property: JsonPropertyName("reason")] string Reason);
