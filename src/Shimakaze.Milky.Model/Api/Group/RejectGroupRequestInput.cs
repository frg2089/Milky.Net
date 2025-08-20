using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Group;

public sealed record class RejectGroupRequestInput(
    [property: JsonPropertyName("notification_seq")] long NotificationSeq,
    [property: JsonPropertyName("is_filtered")] bool IsFiltered = false,
    [property: JsonPropertyName("reason")] string? Reason = null
);
