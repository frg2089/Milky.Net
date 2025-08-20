using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Message;

public sealed record class IncomingXmlSegmentData(
    [property: JsonPropertyName("service_id")] int ServiceId,
    [property: JsonPropertyName("xml_payload")] string XmlPayload
);
