using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Group;

public sealed record class AcceptGroupRequestInput(
    [property: JsonPropertyName("notification_seq")] long NotificationSeq,
    [property: JsonPropertyName("is_filtered")] bool IsFiltered = false
);
