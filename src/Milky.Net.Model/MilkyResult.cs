using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Milky.Net.Model;

public sealed record class MilkyResult(
    [property: JsonPropertyName("status")] MilkyResultStatus Status,
    [property: JsonPropertyName("retcode")] int ReturnCode,
    [property: JsonPropertyName("data")] JsonElement? Data,
    [property: JsonPropertyName("message")] string? Message)
{
    private const string MetadataFactoryRequiresUnreferencedCode = "此方法与 AOT 不兼容。";

    /// <summary>
    /// 反序列化 <see cref="Data"/> 为指定类型。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="typeInfo"></param>
    /// <returns></returns>
    /// <exception cref="MilkyException"></exception>
    /// <exception cref="JsonException"></exception>
    public T GetResult<T>(JsonTypeInfo<T> typeInfo)
    {
        if (Status is not MilkyResultStatus.Ok)
            throw new MilkyException(Message ?? "MilkyResult is not ok.");

        if (Data is null)
            throw new MilkyException(Message ?? "MilkyResult.Data is null.");

        return Data.Value.Deserialize(typeInfo)
            ?? throw new JsonException(Message ?? $"Cannot deserialize string as {typeInfo.Type.FullName}");
    }

    /// <inheritdoc cref="GetResult{T}(JsonTypeInfo{T})"/>
    [RequiresUnreferencedCode(MetadataFactoryRequiresUnreferencedCode)]
    [RequiresDynamicCode(MetadataFactoryRequiresUnreferencedCode)]
    public T GetResult<T>()
    {
        if (Status is not MilkyResultStatus.Ok)
            throw new MilkyException(Message ?? "MilkyResult is not ok.");

        if (Data is null)
            throw new MilkyException(Message ?? "MilkyResult.Data is null.");

        return Data.Value.Deserialize<T>()
            ?? throw new JsonException(Message ?? $"Cannot deserialize string as {typeof(T).FullName}");
    }
}
