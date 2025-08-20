using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

public sealed record class IncomingLightAppSegmentData(
    [property: JsonPropertyName("app_name")] string AppName,
    [property: JsonPropertyName("json_payload")] string JsonPayload
);
