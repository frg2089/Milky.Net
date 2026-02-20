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
    /// <summary>
    /// 反序列化 <see cref="Data"/> 为 <typeparamref name="TResult"/>。
    /// </summary>
    /// <typeparam name="TResult">目标类型</typeparam>
    /// <param name="options">反序列化选项</param>
    /// <returns><typeparamref name="TResult"/>实例</returns>
    /// <exception cref="MilkyException">API 错误</exception>
    /// <exception cref="NotSupportedException">找不到 <typeparamref name="TResult"/> 的反序列化信息</exception>
    /// <exception cref="JsonException">反序列化错误</exception>
    public TResult GetResult<TResult>(JsonSerializerOptions? options = default)
    {
        options ??= MilkyJsonSerializerContext.Default.Options;

        if (options.GetTypeInfo(typeof(TResult)) is not JsonTypeInfo<TResult> typeInfo)
            throw new NotSupportedException($"Cannot resolve type {typeof(TResult).FullName} from JsonSerializerOptions");

        return GetResult(typeInfo);
    }

    /// <summary>
    /// 反序列化 <see cref="Data"/> 为 <typeparamref name="TResult"/>。
    /// </summary>
    /// <typeparam name="TResult">目标类型</typeparam>
    /// <param name="typeInfo"><typeparamref name="TResult"/> 的反序列化信息</param>
    /// <returns><typeparamref name="TResult"/>实例</returns>
    /// <exception cref="MilkyException">API 错误</exception>
    /// <exception cref="JsonException">反序列化错误</exception>
    public TResult GetResult<TResult>(JsonTypeInfo<TResult> typeInfo)
    {
        if (Status is not MilkyResultStatus.Ok)
            throw new MilkyException(Message ?? "MilkyResult is not ok.");

        if (Data is null)
            throw new MilkyException(Message ?? "MilkyResult.Data is null.");

        return Data.Value.Deserialize(typeInfo)
            ?? throw new JsonException(Message ?? $"Cannot deserialize string as {typeof(TResult).FullName}");
    }
}
