using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

public sealed record class TextSegmentData(
    [property: JsonPropertyName("text")] string Text
);
