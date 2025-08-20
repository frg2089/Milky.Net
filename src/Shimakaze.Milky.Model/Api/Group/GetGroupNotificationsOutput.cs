using System.Text.Json.Serialization;

using Shimakaze.Milky.Model.Common;

namespace Shimakaze.Milky.Model.Api.Group;

public sealed record class GetGroupNotificationsOutput(
    [property: JsonPropertyName("notifications")] GroupNotification[] Notifications,
    [property: JsonPropertyName("next_notification_seq")] long? NextNotificationSeq = null
);
