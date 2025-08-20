using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

public sealed record class IncomingMarketFaceSegmentData(
    [property: JsonPropertyName("url")] string Url
);
