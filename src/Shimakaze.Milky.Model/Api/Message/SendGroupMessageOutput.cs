using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Message;

public sealed record class SendGroupMessageOutput(
    [property: JsonPropertyName("message_seq")] long MessageSeq,
    [property: JsonPropertyName("time")][property: JsonConverter(typeof(SecondTimestampDateTimeOffsetJsonConverter))] DateTimeOffset Time
);
