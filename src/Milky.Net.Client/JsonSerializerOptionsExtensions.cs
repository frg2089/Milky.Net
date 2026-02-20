using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Milky.Net.Client;

internal static class JsonSerializerOptionsExtensions
{
    public static JsonTypeInfo<T> GetTypeInfo<T>(this JsonSerializerOptions? options)
    {
        if (options?.GetTypeInfo(typeof(T)) is not JsonTypeInfo<T> typeInfo)
            throw new NotSupportedException($"Cannot resolve type {typeof(T).FullName} from JsonSerializerOptions");

        return typeInfo;
    }
}