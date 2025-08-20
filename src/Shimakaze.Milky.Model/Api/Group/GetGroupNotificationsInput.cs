using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Group;

public sealed record class GetGroupNotificationsInput(
    [property: JsonPropertyName("start_notification_seq")] long? StartNotificationSeq = null,
    [property: JsonPropertyName("is_filtered")] bool IsFiltered = false,
    [property: JsonPropertyName("limit")] int Limit = 20
);
