using System.Text.Json;
using System.Text.Json.Serialization;

namespace Milky.Net.Model;

public sealed record class MilkyResult(
    [property: JsonPropertyName("status")] MilkyResultStatus Status,
    [property: JsonPropertyName("retcode")] int ReturnCode,
    [property: JsonPropertyName("data")] JsonElement? Data,
    [property: JsonPropertyName("message")] string? Message)
{
    /// <summary>
    /// 反序列化 <see cref="Data"/> 为指定类型。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="MilkyException"></exception>
    /// <exception cref="JsonException"></exception>
    public T GetResult<T>()
    {
        if (Status is not MilkyResultStatus.Ok)
            throw new MilkyException(Message ?? "MilkyResult is not ok.");

        if (Data is null)
            throw new MilkyException(Message ?? "MilkyResult.Data is null.");

        var typeInfo = MilkyJsonSerializerContext.Default.GetTypeInfo(typeof(T))
            ?? throw new NotSupportedException($"Type {typeof(T).FullName} are not supported.");

        return (T?)Data.Value.Deserialize(typeInfo)
            ?? throw new JsonException(Message ?? $"Cannot deserialize string as {typeof(T).FullName}");
    }
}
