using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Shimakaze.Milky.Model;

public sealed record class MilkyResult(
    [property: JsonPropertyName("status")] MilkyResultStatus Status,
    [property: JsonPropertyName("retcode")] int ReturnCode,
    [property: JsonPropertyName("data")] JsonElement? Data,
    [property: JsonPropertyName("message")] string? Message)
{
    public T GetResult<T>(JsonTypeInfo<T> typeInfo)
    {
        if (Status is not MilkyResultStatus.Ok)
            throw new MilkyException(Message ?? "MilkyResult is not ok.");

        if (Data is null)
            throw new MilkyException(Message ?? "MilkyResult.Data is null.");

        return Data.Value.Deserialize(typeInfo)
            ?? throw new JsonException(Message ?? $"Cannot deserialize string as {typeInfo.Type.FullName}");
    }
}
