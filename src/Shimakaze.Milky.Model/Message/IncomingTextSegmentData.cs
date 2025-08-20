using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

public sealed record class IncomingTextSegmentData(
    [property: JsonPropertyName("text")] string Text
);
